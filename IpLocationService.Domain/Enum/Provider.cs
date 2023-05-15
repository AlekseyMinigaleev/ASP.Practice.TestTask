namespace IpLocationService.Domain.Enum
{
    /// <summary>
    /// Содержит значение определяющие, сторонний сервис для получения ифнормации о местоположении ip-адреса
    /// </summary>
    public enum Provider
    {
        /// <summary>
        /// ссылка на сервис: <see href="https://ipgeolocation.io">ipgeolocation</see> 
        /// </summary>
        Ipgeolocation = 1,

        /// <summary>
        /// ссылка на сервис: <see href="https://ipinfo.io/">Ipinfo</see> 
        /// </summary>
        Ipinfo = 2,
    }
}
