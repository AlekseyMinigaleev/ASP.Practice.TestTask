using IpLocationService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IpLocationService.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<IpAdressLocation> IpAdressLocations { get; set; }

        public ApplicationDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            /*TODO ???*/
            var builder = new ConfigurationBuilder();
            // установка пути к текущему каталогу
            builder.SetBasePath(Directory.GetCurrentDirectory());
            // получаем конфигурацию из файла appsettings.json
            builder.AddJsonFile("appsettings.json");
            // создаем конфигурацию
            var config = builder.Build();

            var connectionString = config.GetConnectionString("SqliteConnection");
            optionsBuilder.UseSqlite(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<IpRequest>();
            modelBuilder.Entity<IpAdressLocation>()
              .HasKey(_ => _.Id);
        }
    }
}