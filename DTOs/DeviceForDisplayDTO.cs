using System.ComponentModel.DataAnnotations;

namespace AgroMonitor.DTOs
{
    public class DeviceForDisplayDTO
    {
        public int Id { get; set; }
        public string BrandCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}
