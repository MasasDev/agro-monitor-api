using System.ComponentModel.DataAnnotations;

namespace AgroMonitor.DTOs
{
    public class AddDeviceForDisplayDTO
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string BrandCode { get; set; } = null!;
    }
}
