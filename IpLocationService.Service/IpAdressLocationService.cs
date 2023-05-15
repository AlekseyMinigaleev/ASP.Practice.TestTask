using Azure.Core;
using IpLocationService.DAL.Repositories;
using IpLocationService.Domain.Entity;
using IpLocationService.Domain.Enum;
using IpLocationService.Domain.Responce;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;

namespace IpLocationService.Service
{

    /// <summary>
    /// Класс для опредления местоположения ip-адреса
    /// </summary>
    public class IpAdressLocationService
    {
        private const string IPINFO_TOKEN = "d9dd53b48541ee";
        private const string IPGEOLOCATION_TOKEN = "b45b2541d9a94d7894e1e0f7dd029a71";
        private readonly IpAdressLocationRepository adressLocationRepository;

        /// <summary>
        /// Создает новый объект <see cref="IpAdressLocationService"/> c параметром <paramref name="adressLocationRepository"/>.
        /// </summary>
        /// <param name="adressLocationRepository">Репозиторий для работы с базой данных</param>
        public IpAdressLocationService(IpAdressLocationRepository adressLocationRepository)
        {
            this.adressLocationRepository = adressLocationRepository;
        }

        /*TODO тут хуйня с ServiceResponse<T>*/
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
                var isExistInDatatbase = await adressLocationRepository.IsExistByIpRequestAsync(infoForRequest);
                if (isExistInDatatbase)
                {
                    ipAddressLocation = (await adressLocationRepository.GetByIpRequestAsync(infoForRequest))!;
                }
                else
                {
                    ipAddressLocation = await GetIpLocationFromProviderAsync(infoForRequest);
                    await adressLocationRepository.AddAsync(ipAddressLocation);
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
        ///  Проверяет, является ли указанный <paramref name="provider"/> допустимым
        /// </summary>
        /// <param name="provider"><see cref="Provider"/>, который следует проверить</param>
        /// <returns>
        /// <see langword="true"/> если <paramref name="provider"/> допустимый; иначе <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// <paramref name="provider"/> является не допустимым, если имеет значение 0.
        /// </remarks>
        public bool IsValidProvider(Provider provider)
        {
            if (provider == 0)
                return false;
            return Enum.IsDefined(typeof(Provider), provider);
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

        private async Task<IpAddressLocation> GetIpLocationFromProviderAsync(IpRequest request)
        {
            var (apiUrl, parametrs) = GetDataForRequestToProvider(request);
            var providerResponse = await GetResponseFromProviderAsync(apiUrl);
            var ipAddressLocation = await ConvertResponseToIpAdressLocation(providerResponse, parametrs);
            ipAddressLocation.Provider = request.Provider;

            if (ipAddressLocation is null)
                throw new Exception("Location was not determined");

            return ipAddressLocation;
        }

        /*TODO hardCode*/
        private static (string, Dictionary<string, string>) GetDataForRequestToProvider(IpRequest request)
        {
            switch (request.Provider)
            {
                case Provider.Ipgeolocation:
                    {
                        var fieldMappings = new Dictionary<string, string>
                            {
                                {"Ip", "ip"},
                                {"City", "city"},
                                {"Region", "state_prov"},
                                {"Country", "country_name"},
                                {"Timezone", "time_zone.name"},
                            };
                        var apiUrl = $"https://api.ipgeolocation.io/ipgeo?apiKey={IPGEOLOCATION_TOKEN}&ip={request.Ip}";

                        return (apiUrl, fieldMappings);
                    }
                case Provider.Ipinfo:
                    {
                        var fieldMappings = new Dictionary<string, string>
                            {
                                {"Ip", "ip"},
                                {"City", "city"},
                                {"Region", "region"},
                                {"Country", "country"},
                                {"Timezone", "timezone"},
                            };
                        var apiUrl = $"https://ipinfo.io/{request.Ip}?token={IPINFO_TOKEN}";
                        return (apiUrl, fieldMappings);
                    }
                default:
                    throw new Exception("Invalid provider");
            }
        }

        private static async Task<HttpResponseMessage> GetResponseFromProviderAsync(string apiUrl)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(apiUrl);
            return response;
        }

        /*TODO ??? */
        #region ne ponyatno nihuya
        private async Task<IpAddressLocation> ConvertResponseToIpAdressLocation(HttpResponseMessage response, Dictionary<string, string> parameters)
        {
            var jsonContent = await response.Content.ReadAsStringAsync();
            dynamic jObject = JsonConvert.DeserializeObject(jsonContent);

            var locationResponse = new IpAddressLocation();
        
            foreach (var parameter in parameters)
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
        #endregion
    }
}