using System.ComponentModel.DataAnnotations;

namespace AgroMonitor.DTOs
{
    public class UpdateDeviceForDisplayDTO
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string BrandCode { get; set; } = null!;
    }
}
