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
            if (request is null)
                return new JsonResult("Null exception");

            if (request.Provider == 0)
                return new JsonResult("Null provider exception");

            /*TODO change to ErrorMessage*/
            if (request.Ip is null)
            {
                var response = await ipAdressLocationService.GetAllAsync();
                return new JsonResult(response.Result);
            }

            if (!ipAdressLocationService.IsValidIp(request.Ip))
                return Content("Invalid IP");

            var responce = await ipAdressLocationService.GetAsync(request);
            if (responce.StatusCode == Domain.Enum.StatusCode.Ok)
                return new JsonResult(responce.Result);

            /*TODO how is view ErrorMessage was been returning*/
            return Content(responce.Message!);
        }

        /*TODO country code to country name*/
    }
}