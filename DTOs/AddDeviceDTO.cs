using System.ComponentModel.DataAnnotations;

namespace AgroMonitor.DTOs
{
    public class AddDeviceDTO()
    {
        [Required]
        public string BrandCode { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string DeviceIdentifier { get; set; } = null!;
    }
}
