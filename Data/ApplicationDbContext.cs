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
        public DbSet<Package> Packages { get; set; }
        public DbSet<CustomerPackage> CustomerPackages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>().HasIndex(c => c.CustomerUniqueIdentifier).IsUnique();

            modelBuilder.Entity<CustomerPackage>().HasIndex(cp => cp.Id);

            modelBuilder.Entity<CustomerPackage>()
                .HasOne(cp => cp.Customer)
                .WithMany(c => c.CustomerPackages)
                .HasForeignKey(cp => cp.CustomerId);

            modelBuilder.Entity<CustomerPackage>()
                .HasOne(cp => cp.Package)
                .WithMany(p => p.CustomerPackages)
                .HasForeignKey(cp => cp.PackageId);
        }
    }
}
