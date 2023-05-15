namespace IpLocationService.Domain.Entity
{

    /// <summary>
    /// Класс, описывающий конфигурацию провайдеров.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Словарь провайдеров, где ключом является название провайдера, а значением - его конфигурация.
        /// </summary>
        public Dictionary<string, ProviderConfig> Providers { get; set; }
    }
}
