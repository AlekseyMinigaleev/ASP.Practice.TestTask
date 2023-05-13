namespace IpLocationService.Domain.Entity
{
    public class IpAdressLocation
    {
        public string Ip { get; set; } = null!;

        public string? City { get; set; }

        public string? Region { get; set; }

        public string? Country { get; set; }

        public string? Timezone { get; set; }
    }
}
