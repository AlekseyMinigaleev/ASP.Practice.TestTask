using IpLocationService.DAL.Repositories;
using IpLocationService.Domain.Entity;
using IpLocationService.Domain.Enum;
using IpLocationService.Domain.Responce;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;

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

        public async Task<Response<IpAdressLocation>> GetAsync(IpRequest request)
        {
            try
            {
                IpAdressLocation ipAddressLocation;
                var isExistInDatatbase = await adressLocationRepository.IsExistByIpRequestAsync(request);
                if (isExistInDatatbase)
                {
                    ipAddressLocation = (await adressLocationRepository.GetByIpRequestAsync(request))!;
                }
                else
                {
                    ipAddressLocation = await GetIpLocationFromProviderAsync(request);
                    await adressLocationRepository.AddAsync(ipAddressLocation);
                }

                return new Response<IpAdressLocation>
                {
                    Result = ipAddressLocation,
                    StatusCode = StatusCode.Ok,
                };
            }
            catch (Exception e)
            {
                return new Response<IpAdressLocation>
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
            var ipAddressLocation = await ConvertResponseToIpAdressLocation(providerResponse, parametrs);
            ipAddressLocation.Provider = request.Provider;

            if (ipAddressLocation is null)
                throw new Exception("Location was not determined");

            return ipAddressLocation;
        }

        /*TODO TESTS*/
        public static bool IsValidIp(string ip)
        {
            if(ip is null)
                return false;
            var  pattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            return Regex.IsMatch(ip, pattern);
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

        /*TODO ???*/
        #region ne ponyatno nihuya
        private async Task<IpAdressLocation> ConvertResponseToIpAdressLocation(HttpResponseMessage response, Dictionary<string, string> parameters)
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
    }
}