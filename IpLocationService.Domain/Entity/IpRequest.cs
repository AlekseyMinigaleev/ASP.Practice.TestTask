using IpLocationService.Domain.Enum;

namespace IpLocationService.Domain.Entity
{
    public class IpRequest
    {
        public string Ip { get; set; }
        public Provider Provider { get; set; }

        public IpRequest(string ip, Provider provider)
        {
            Ip = ip;
            Provider = provider;
        }
    }
}
