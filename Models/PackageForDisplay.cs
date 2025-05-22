using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroMonitor.Models
{
    public class PackageForDisplay
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }


        [ForeignKey(nameof(DeviceForDisplay))]
        public int DeviceForDisplayId { get; set; }
        public DeviceForDisplay DeviceForDisplay { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}
