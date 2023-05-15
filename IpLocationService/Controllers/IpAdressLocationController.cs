using IpLocationService.Domain.Entity;
using IpLocationService.Service;
using Microsoft.AspNetCore.Mvc;

namespace IpLocationService.Controllers
{
    /// <summary>
    /// API-контроллер для получения информации о местоположении IP-адреса.
    /// </summary>
    /// <remarks>
    /// Контроллер предоставляет POST/GET HTTP метод для получения информации о местоположении IP-адреса в формате <see cref="JsonResult"/>.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class IpAdressLocationController : ControllerBase
    {
        private readonly IpAdressLocationService ipAdressLocationService;

        /// <summary>
        /// Создает новый объект <see cref="IpAdressLocationController"/>  c параметром <paramref name="ipAdressLocationService"/>.
        /// </summary>
        /// <param name="ipAdressLocationService">объект <see cref="IpAdressLocationService"/>, определяющий местоположение IP-адреса</param>
        public IpAdressLocationController(IpAdressLocationService ipAdressLocationService)
        {
            this.ipAdressLocationService = ipAdressLocationService;
        }

        /// <summary>
        /// Возвращает информацию о местоположении <paramref name="ipAddress"/> от выбранного <paramref name="provider"/>
        /// </summary>
        /// <param name="ipAddress">IP-адрес, местоположение которого нужно найти.</param>
        /// <param name="provider">Провайдер, который будет использоваться для получения информации о местоположении IP-адреса.</param>
        /// <returns><see cref="JsonResult"/> с информацией о местоположении <paramref name="ipAddress"/></returns>
        /// <remarks>
        /// Если <paramref name="ipAddress"/> или <paramref name="provider"/> является не допустимым, возвращает сообщение об ошибке.
        /// </remarks>
        [HttpGet]
        public JsonResult GetIpLocation([FromQuery] string ipAddress,[FromQuery] int provider) =>
            MainLogicAsync(new IpRequest(ipAddress,provider)).Result;

        /// <summary>
        /// Возвращает информацию о местоположении <paramref name="infoForRequest"/>
        /// </summary>
        /// <returns>Задача представляющая ассинхронную опреацию. Результат задачи <see cref="JsonResult"/> c информацией о местоположении <paramref name="infoForRequest"/></returns>
        /// <remarks>
        /// Если <paramref name="infoForRequest"/> является не допустимым, возвращает сообщение об ошибке.
        /// </remarks>
        [HttpPost]
        public IActionResult GetIpLocation([FromBody] IpRequest infoForRequest) =>
            MainLogicAsync(infoForRequest).Result;

        private async Task<JsonResult> MainLogicAsync (IpRequest infoForRequest)
        {
            if (!ipAdressLocationService.IsValidIp(infoForRequest.Ip))
                return new JsonResult("Invalid IP");

            var responce = await ipAdressLocationService.GetAsync(infoForRequest);

            if ((int)responce.StatusCode<400)
                return new JsonResult(new 
                {
                    responce.Result.Ip,
                    responce.Result.City,
                    responce.Result.Region,
                    responce.Result.Country,
                    responce.Result.Timezone,
                });

            return new JsonResult(responce.ErrorMessage!);
        }
    }
}