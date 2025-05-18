using AgroMonitor.Models;
using Microsoft.EntityFrameworkCore;

namespace AgroMonitor.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Device> Devices { get; set; }
        public DbSet<SensorReading> SensorReadings { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>().HasIndex(c => c.CustomerUniqueIdentifier).IsUnique();
        }
    }
}
