using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroMonitor.Models
{
    public class Package
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public decimal Price { get; set; }

        [ForeignKey(nameof(Device))]
        public int DeviceId { get; set; }
        public Device Device { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }

        public ICollection<CustomerPackage> CustomerPackages { get; set; } = new List<CustomerPackage>();

    }
}
