using System.Net;

namespace IpLocationService.Domain.Responce
{
    /// <summary>
    /// Класс, представляющий ответ на запрос от сервиса.
    /// </summary>
    /// <typeparam name="T">Тип объекта, который содержится в ответе.</typeparam>
    public class ServiceResponse<T>
    {
        /// <summary>
        /// Сообщение об ошибке, если такая произошла во время выполнения запроса.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Код статуса ответа.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Результат выполнения запроса.
        /// </summary>
        public T? Result { get; set; }
    }
}
