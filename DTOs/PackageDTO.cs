using System.ComponentModel.DataAnnotations;
using AgroMonitor.Models;

namespace AgroMonitor.DTOs
{
    public class PackageDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int DeviceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public List<CustomerDTO> Customers { get; set; } = new();
    }
}
