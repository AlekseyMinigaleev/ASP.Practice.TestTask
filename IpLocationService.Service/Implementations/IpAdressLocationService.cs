using IpLocationService.DAL.Repositories;
using IpLocationService.Domain.Entity;
using IpLocationService.Domain.Enum;
using IpLocationService.Domain.Responce;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace IpLocationService.Service.Implementations
{
    public class IpAdressLocationService
    {
        private const string IPINFO_TOKEN = "d9dd53b48541ee";
        private const string IPGEOLOCATION_TOKEN = "b45b2541d9a94d7894e1e0f7dd029a71";
        private readonly IpAdressLocationRepository adressLocationRepository;

        public IpAdressLocationService(IpAdressLocationRepository adressLocationRepository)
        {
            this.adressLocationRepository = adressLocationRepository;
        }

        /*Usless*/
        public async Task<BaseResponse<IEnumerable<IpAdressLocation>>> GetAllAsync()
        {
            try
            {
                var locationResponses = await adressLocationRepository.GetAllAsync();

                if (locationResponses.Count == 0)
                    return new BaseResponse<IEnumerable<IpAdressLocation>>
                    {
                        Message = "DataBase is empty",
                        StatusCode = StatusCode.NotFount,
                    };

                return new BaseResponse<IEnumerable<IpAdressLocation>>
                {
                    Result = locationResponses,
                    StatusCode  = StatusCode.Ok
                };
            }
            /*TODO mb rework*/
            catch (Exception e)
            {
                return new BaseResponse<IEnumerable<IpAdressLocation>>()
                {
                    Message = $"[GetAllAsync]: {e.Message}",
                    StatusCode = StatusCode.Error
                };
            }
        }

        public async Task<BaseResponse<IpAdressLocation>> GetAsync(IpRequest request)
        {
            try
            {
                IpAdressLocation ipAddressLocation;
                var isExist = await adressLocationRepository.IsExistByIpAsync(request.Ip);
                if (isExist)
                {
                    ipAddressLocation = (await adressLocationRepository.GetByIpAsync(request.Ip))!;
                }
                else
                {
                    ipAddressLocation = await GetIpLocationFromProviderAsync(request);
                    await adressLocationRepository.AddAsync(ipAddressLocation);
                }

                return new BaseResponse<IpAdressLocation>
                {
                    Result = ipAddressLocation,
                    StatusCode = StatusCode.Ok,
                };
            }
            catch (Exception e)
            {
                return new BaseResponse<IpAdressLocation>
                {
                    Message = e.Message,
                    StatusCode  = StatusCode.Error,
                };
            }
        }

        private async Task<IpAdressLocation> GetIpLocationFromProviderAsync(IpRequest request)
        {
            var (apiUrl, parametrs) = GetDataForRequestToProvider(request);
            var providerResponse = await GetResponseFromProviderAsync(apiUrl);
            var ipAddressLocation = await ConvertResponseToLocationResponseAsync(providerResponse, parametrs);

            if (ipAddressLocation is null)
                throw new Exception("location was not determined");

            return ipAddressLocation;
        }

        #region Utils
        /*TODO TESTS*/
        public bool IsValidIp(string ip)
        {
            if(ip is null)
                return false;

            return IPAddress.TryParse(ip, out _);
        }

        /*TODO hardCode*/
        private static (string, Dictionary<string, string>) GetDataForRequestToProvider(IpRequest request)
        {
            switch (request.Provider)
            {
                case Provider.IpGeolocation:
                    {
                        var parameters = new Dictionary<string, string>
                            {
                                {"Ip", "ip"},
                                {"City", "city"},
                                {"Region", "state_prov"},
                                {"Country", "country_name"},
                                {"Timezone", "time_zone.name"},
                            };
                        var apiUrl = $"https://api.ipgeolocation.io/ipgeo?apiKey={IPGEOLOCATION_TOKEN}&ip={request.Ip}";

                        return (apiUrl, parameters);
                    }
                case Provider.IpInfo:
                    {
                        var parameters = new Dictionary<string, string>
                            {
                                {"Ip", "ip"},
                                {"City", "city"},
                                {"Region", "region"},
                                {"Country", "country"},
                                {"Timezone", "timezone"},
                            };
                        var apiUrl = $"https://ipinfo.io/{request.Ip}?token={IPINFO_TOKEN}";
                        return (apiUrl, parameters);
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

        /*TODO ???*/
        #region ne ponyatno nihuya
        private async Task<IpAdressLocation> ConvertResponseToLocationResponseAsync(HttpResponseMessage response, Dictionary<string, string> parameters)
        {
            var jsonContent = await response.Content.ReadAsStringAsync();
            dynamic jObject = JsonConvert.DeserializeObject(jsonContent);

            var locationResponse = new IpAdressLocation();

            foreach (var parameter in parameters)
            {
                var propertyInfo = typeof(IpAdressLocation).GetProperty(parameter.Key);
                if (propertyInfo != null)
                {
                    var value = GetValueFromJObject(jObject, parameter.Value);
                    propertyInfo.SetValue(locationResponse, value);
                }
            }

            return locationResponse;
        }

        private static string? GetValueFromJObject(dynamic jObject, string propertyName)
        {
            var propertyNames = propertyName.Split('.');
            var result = jObject;

            foreach (var name in propertyNames)
            {
                if (result == null)
                    return null;

                result = result[name];
            }

            return result?.ToString();
        }
        #endregion
        #endregion
    }
}