using System.ComponentModel.DataAnnotations;

namespace AgroMonitor.DTOs
{
    public class UpdatePackageForDisplayDTO
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, long.MaxValue)]
        public int DeviceForDisplayId { get; set; }
    }
}
