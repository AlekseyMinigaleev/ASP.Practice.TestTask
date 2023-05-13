using IpLocationService.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace IpLocationService.DAL.Repositories
{
    public class IpAdressLocationRepository 
    {
        private readonly ApplicationDbContext db;

        public IpAdressLocationRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<List<IpAdressLocation>> GetAllAsync() =>
            await db.IpAdressLocations.ToListAsync();

        public async Task<IpAdressLocation?> GetByIpAsync(string ip) =>
            await db.IpAdressLocations.FirstOrDefaultAsync(_ => _.Ip == ip);

        public async Task AddAsync(IpAdressLocation ipAdressLocation)
        {
            await db.AddAsync(ipAdressLocation);
            await db.SaveChangesAsync();
        }

        public async Task<bool> IsExistByIpAsync(string ip)=>
            await db.IpAdressLocations.AnyAsync(_ => _.Ip == ip);
    }
}