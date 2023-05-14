
using IpLocationService.Domain.Enum;

namespace IpLocationService.Domain.Responce
{
    /*TODO Naming*/
    public class Response<T>
    {
        public string? Message { get; set; }
        public StatusCode StatusCode { get; set; }
        public T? Result { get; set; }
    }
}
