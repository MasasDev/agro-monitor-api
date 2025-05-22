using System.ComponentModel.DataAnnotations;

namespace AgroMonitor.DTOs
{
    public class UpdateDeviceDTO
    {
        public int Id { get; set; }
        [Required]
        public string BrandCode { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;
        public string? DeviceIdentifier { get; set; }
    }
}
