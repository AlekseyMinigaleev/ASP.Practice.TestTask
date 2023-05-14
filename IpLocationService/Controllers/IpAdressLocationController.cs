using IpLocationService.Domain.Entity;
using IpLocationService.Domain.Enum;
using IpLocationService.Service.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace IpLocationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IpAdressLocationController : ControllerBase
    {
        private readonly IpAdressLocationService ipAdressLocationService;

        public IpAdressLocationController(IpAdressLocationService ipAdressLocationService)
        {
            this.ipAdressLocationService = ipAdressLocationService;
        }

        [HttpGet]
        public IActionResult GetIpLocation([FromQuery] string ip,[FromQuery] Provider provider) =>
            MainLogic(new IpRequest(ip,provider)).Result;

        [HttpPost]
        public IActionResult GetIpLocation([FromBody] IpRequest request) =>
            MainLogic(request).Result;

        private async Task<IActionResult> MainLogic (IpRequest request)
        {
            if (!IpAdressLocationService.IsValidIp(request.Ip))
                return new JsonResult("Invalid IP");

            if (request.Provider == 0)
                return new JsonResult("provider is required fill");

            if (!Enum.IsDefined(typeof(Provider), request.Provider))
                return new JsonResult("Invalid provider");

            var responce = await ipAdressLocationService.GetAsync(request);
            if (responce.StatusCode == Domain.Enum.StatusCode.Ok)
                return new JsonResult(new 
                {
                    responce.Result.Ip,
                    Provider = responce.Result.Provider.ToString(),
                    responce.Result.City,
                    responce.Result.Region,
                    responce.Result.Country,
                    responce.Result.Timezone,
                });

            return new JsonResult(responce.Message!);
        }
    }
}