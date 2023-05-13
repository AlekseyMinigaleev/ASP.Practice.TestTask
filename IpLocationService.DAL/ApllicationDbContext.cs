using IpLocationService.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace IpLocationService.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<IpAdressLocation> IpAdressLocations { get; set; }

        public ApplicationDbContext()
        {
            Database.EnsureCreated();
        }

        /*TODO hardcode*/
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-VBHTIR3;Database=IPLocationService;Trusted_Connection=True;TrustServerCertificate=Yes");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<IpRequest>();
            modelBuilder.Entity<IpAdressLocation>()
              .HasKey(_ => _.Ip);
        }
    }
}