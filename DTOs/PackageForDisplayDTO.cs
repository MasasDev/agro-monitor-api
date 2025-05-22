using AgroMonitor.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroMonitor.DTOs
{
    public class PackageForDisplayDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public DeviceForDisplayDTO DeviceForDisplay { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}
