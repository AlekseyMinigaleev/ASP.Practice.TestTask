using IpLocationService.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace IpLocationService.DAL.Repositories
{
    /// <summary>
    /// Класс, представляющий репозиторий для работы с объектами типа <see cref="IpAddressLocation"/> в базе данных.
    /// </summary>
    public class IpAddressLocationRepository 
    {
        private readonly ApplicationDbContext db;

        /// <summary>
        /// Создает новый экземпляр класса <see cref="IpAddressLocationRepository"/>, используя указанный контекст базы данных.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        public IpAddressLocationRepository(ApplicationDbContext dbContext)
        {
            db = dbContext;
        }

        /// <summary>
        /// возвращает перечисление объектов типа <see cref="IpAddressLocation"/> из базы данных.
        /// </summary>
        /// <returns><see cref="IEnumerable{IpAddressLocation}"/> объектов <see cref="IpAddressLocation"/>.</returns>
        public IEnumerable<IpAddressLocation> GetAll() =>
            db.IpAdressLocations;

        /// <summary>
        /// Ассинхронно возвращает <see cref="IpAddressLocation"/> из базы данных по запросу <paramref name="request"/>.
        /// </summary>
        /// <param name="request">Информация для получения местоположения IP-адреса.</param>
        /// <returns>Задача представляющая ассинхронную опреацию. Результат задачи <see cref="IpAddressLocation"/> или <see langword="null"/>, если объект не найден.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        public async Task<IpAddressLocation?> GetByIpRequestAsync(IpRequest request) =>
        await db.IpAdressLocations.FirstOrDefaultAsync(_ => _.Ip == request.Ip && _.ProviderId == request.ProviderId);


        /// <summary>
        /// Ассинхронно Добавляет <paramref name="ipAdressLocation"/> в базу данных.
        /// </summary>
        /// <param name="ipAdressLocation">Добавляемый объект типа <see cref="IpAddressLocation"/>.</param>
        /// <returns>Задача, представляющая добавление <paramref name="ipAdressLocation"/> в базу данных.</returns>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="DbUpdateException"></exception>
        /// <exception cref="DbUpdateConcurrencyException"></exception>
        public async Task AddAsync(IpAddressLocation ipAdressLocation)
        {
            await db.AddAsync(ipAdressLocation);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Ассинхронно проверяет, существует ли объект <see cref="IpAddressLocation"/> в базе данных с данными <paramref name="request"/>.
        /// </summary>
        /// <param name="request">данные для получение информации о местоположении IP-адреса.</param>
        /// <returns>Задача представляющая ассинхронную опреацию. Результат задачи <see langword="true"/>, если объект существует в базе данных, иначе - <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        public async Task<bool> IsExistByIpRequestAsync(IpRequest request)=>
            await db.IpAdressLocations.AnyAsync(_ => _.Ip == request.Ip && _.ProviderId == request.ProviderId);
    }
}