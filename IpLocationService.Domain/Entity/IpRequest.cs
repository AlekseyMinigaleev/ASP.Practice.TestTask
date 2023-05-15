using IpLocationService.Domain.Enum;

namespace IpLocationService.Domain.Entity
{
    /// <summary>
    /// Представляет данные для получение информации о местоположении IP-адреса.
    /// </summary>
    public class IpRequest
    {
        /// <summary>
        /// Получает или задает IP-адрес для запроса информации о местоположении.
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Получает или задает провайдера, которого следует использовать для получения информации о местоположении.
        /// </summary>
        public Provider Provider { get; set; }

        /// <summary>
        /// Содзает новый экземпляр класса <see cref="IpRequest"/> с указанными параметрами.
        /// </summary>
        /// <param name="ip">IP-адрес для запроса информации о местоположении.</param>
        /// <param name="provider">Провайдер, которого следует использовать для получения информации о местоположении.</param>
        public IpRequest(string ip, Provider provider)
        {
            Ip = ip;
            Provider = provider;
        }
    }
}
