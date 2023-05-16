using IpLocationService.DAL.Repositories;
using IpLocationService.Domain.Entity;
using IpLocationService.Domain.Responce;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;

namespace IpLocationService.Service
{
    /// <summary>
    /// Класс для опредления местоположения ip-адреса
    /// </summary>
    public class IpAddressLocationService
    {
        private readonly IpAddressLocationRepository addressLocationRepository;

        /// <summary>
        /// Создает новый объект <see cref="IpAddressLocationService"/> c параметром <paramref name="addressLocationRepository"/>.
        /// </summary>
        /// <param name="addressLocationRepository">Репозиторий для работы с базой данных</param>
        public IpAddressLocationService(IpAddressLocationRepository addressLocationRepository)
        {
            this.addressLocationRepository = addressLocationRepository;
        }

        /// <summary> 
        /// Ассинхронно возвращает <see cref="ServiceResponse{IpAddressLocation}"/> содержащий информацию о местоположении IP-адреса на основе <paramref name="infoForRequest"/>
        /// </summary>
        /// <param name="infoForRequest">Информация для получения местоположения IP-адреса.</param>
        /// <returns>Задача представляющая ассинхронную опреацию. Результат задачи <see cref="ServiceResponse{IpAddressLocation}"/> c  полученной информацией IP-адреса</returns>
        /// <remarks>
        /// Если IP-адрес из <paramref name="infoForRequest"/> не содержится в базе данных, метод делает запрос к сторонниму провайдеру и добавляет результат в базу данных.
        /// В случае ошибки возвращает <see cref="ServiceResponse{IpAddressLocation}"/> с ифнормацие об ошибке
        /// </remarks>
        public async Task<ServiceResponse<IpAddressLocation>> GetAsync(IpRequest infoForRequest)
        {
            try
            {   
                IpAddressLocation ipAddressLocation;
                var isExistInDatatbase = await addressLocationRepository.IsExistByIpRequestAsync(infoForRequest);
                if (isExistInDatatbase)
                {
                    ipAddressLocation = (await addressLocationRepository.GetByIpRequestAsync(infoForRequest))!;
                }
                else
                {
                    ipAddressLocation = await GetIpLocationFromProviderAsync(infoForRequest);
                    await addressLocationRepository.AddAsync(ipAddressLocation);
                }

                return new ServiceResponse<IpAddressLocation>
                {
                    Result = ipAddressLocation,
                    StatusCode = HttpStatusCode.OK,
                };
            }
            catch (Exception e)
            {
                return new ServiceResponse<IpAddressLocation>
                {
                    ErrorMessage = e.Message,
                    StatusCode  = HttpStatusCode.InternalServerError,
                };
            }
        }

        /// <summary>
        ///  Проверяет, является ли указанный <paramref name="ip"/> допустимым
        /// </summary>
        /// <param name="ip"> IP-адресс для проверки</param>
        /// <returns>
        /// <see langword="true"/> если <paramref name="ip"/> допустимый; иначе <see langword="false"/>.
        /// </returns>
        public bool IsValidIp(string ip)
        {
            if (ip is null)
                return false;
            var pattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            return Regex.IsMatch(ip, pattern);
        }

        private async Task<IpAddressLocation> GetIpLocationFromProviderAsync(IpRequest infoForRequest)
        {
            var configJson = File.ReadAllText("Properties\\ProvidersSettings.json");
            if (configJson is null)
                throw new Exception("Provider.Json file is empty");

            var config = JsonConvert.DeserializeObject<Config>(configJson);
            var (apiUrl, parametrs) = GetDataForRequestToProvider(infoForRequest,config);
            var providerResponse = await GetResponseFromProviderAsync(apiUrl);
            var ipAddressLocation = await ConvertResponseToIpAdressLocation(providerResponse, parametrs);
            ipAddressLocation.ProviderId = infoForRequest.ProviderId;

            if (ipAddressLocation is null)
                throw new Exception("Location was not determined");

            return ipAddressLocation;
        }

        private static (string, Dictionary<string, string>) GetDataForRequestToProvider(IpRequest infoForRequest, Config config)
        {
            var providerConfig = config.Providers
                .FirstOrDefault(p => p.Value.Id
                    .Equals(infoForRequest.ProviderId))
                .Value;

            if (providerConfig is null)
                throw new Exception("Invalid provider");

            var apiUrl = $"{string.Format(providerConfig.Url, infoForRequest.Ip)}";

            var fieldMappings = new Dictionary<string, string>
            {
                {"Ip", providerConfig.Ip},
                {"City", providerConfig.City},
                {"Region", providerConfig.Region},
                {"Country", providerConfig.Country},
                {"Timezone", providerConfig.Timezone},
            };

            return (apiUrl, fieldMappings);
        }

        private static async Task<HttpResponseMessage> GetResponseFromProviderAsync(string apiUrl)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(apiUrl);
            return response;
        }

        private async Task<IpAddressLocation> ConvertResponseToIpAdressLocation(HttpResponseMessage response, Dictionary<string, string> fieldMappings)
        {
            var jsonContent = await response.Content.ReadAsStringAsync();
            dynamic jObject = JsonConvert.DeserializeObject(jsonContent)!;

            var locationResponse = new IpAddressLocation();
        
            foreach (var parameter in fieldMappings)
            {
                var propertyInfo = typeof(IpAddressLocation).GetProperty(parameter.Key);
                if (propertyInfo != null)
                    propertyInfo.SetValue(locationResponse, GetValueFromJObject(jObject, parameter.Value));
            }

            return locationResponse;
        }

        private static string? GetValueFromJObject(dynamic jObject, string propertyName)
        {
            var propertyNames = propertyName.Split('.');
            var result = jObject;

            foreach (var name in propertyNames)
            {
                if (result is null)
                    return null;

                result = result[name];
            }

            return result?.ToString();
        }
    }
}