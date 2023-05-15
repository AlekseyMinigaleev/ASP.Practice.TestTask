using IpLocationService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IpLocationService.DAL
{
    /// <summary>
    /// Класс контекста для базы данных приложения.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Свойство, представляющее таблицу записей местоположения IP-адресов.
        /// </summary>
        public DbSet<IpAddressLocation> IpAdressLocations { get; set; }

        /// <summary>
        /// Создает новый экземпляр класса <see cref="ApplicationDbContext"/>.
        /// </summary>
        /// <remarks>В момент создания экземпляра класса, создается база данные, если она не была создана.</remarks>
        public ApplicationDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            /*TODO ???*/
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();

            var connectionString = config.GetConnectionString("SqlServerConntection");
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<IpRequest>();
            modelBuilder.Entity<IpAddressLocation>()
              .HasKey(_ => _.Id);
        }
    }
}