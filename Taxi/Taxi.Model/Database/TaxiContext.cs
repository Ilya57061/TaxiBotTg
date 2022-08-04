using Microsoft.EntityFrameworkCore;
using Taxi.Model.Models;

namespace Taxi.Model.Database
{
    public class TaxiContext:DbContext
    {
        public TaxiContext(DbContextOptions<TaxiContext> options):base(options)
        {
            
        }

      
        public DbSet<Car> Cars { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
