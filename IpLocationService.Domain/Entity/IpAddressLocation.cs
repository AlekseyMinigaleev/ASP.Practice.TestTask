using IpLocationService.Domain.Enum;

namespace IpLocationService.Domain.Entity
{
    /// <summary>
    /// Представляет сущность, описывающаю информацию о местоположении IP-адреса.
    /// </summary>
    public class IpAddressLocation
    {
        /// <summary>
        /// Уникальный идентификатор местоположения IP-адреса в базе данных.
        /// </summary>
        /// <remarks>
        /// Это поле также является первичным ключем таблицы
        /// </remarks>
        public int Id { get;}

        /// <summary>
        /// IP-адрес.
        /// </summary>
        public string Ip { get; set; } = null!;

        /// <summary>
        /// Сторонний сервис, использованный для определения местоположения IP-адреса.
        /// </summary>
        public Provider Provider { get; set; }

        /// <summary>
        /// Город, соответствующий местоположению IP-адреса.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Регион, соответствующий местоположению IP-адреса.
        /// </summary>
        public string? Region { get; set; }

        /// <summary>
        /// Страна, соответствующая местоположению IP-адреса.
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// Временная зона, соответствующая местоположению IP-адреса.
        /// </summary>
        public string? Timezone { get; set; }
    }
}   