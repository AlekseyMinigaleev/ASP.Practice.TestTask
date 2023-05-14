using IpLocationService.Domain.Enum;

namespace IpLocationService.Domain.Entity
{
    public class IpAdressLocation
    {
        public int Id { get; set; }

        public string Ip { get; set; } = null!;

        public Provider Provider { get; set; }

        public string? City { get; set; }

        public string? Region { get; set; }

        public string? Country { get; set; }

        public string? Timezone { get; set; }
    }
}   