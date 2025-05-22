using System.ComponentModel.DataAnnotations;

namespace AgroMonitor.Models
{
    public class DeviceForDisplay
    {
        [Key]
        public int Id {  get; set; }

        [Required]
        public string BrandCode { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}
