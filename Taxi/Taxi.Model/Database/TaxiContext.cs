using Microsoft.EntityFrameworkCore;
using Taxi.Model.Models;

namespace Taxi.Model.Database
{
    public class TaxiContext:DbContext
    {
        public TaxiContext(DbContextOptions<TaxiContext> options):base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Category>().HasData(new Category { Id=1, Name="Эконом", Description="nn", Price=1});
            modelBuilder.Entity<Driver>().HasData(new Driver { Id = 1, DrivingExperience=3, Name = "Александр Гержа"});
            modelBuilder.Entity<Car>().HasData(new Car { Id = 1, CategoryId=1, DriverId=1, Model="VWPolo", Number="3232" });
        }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
