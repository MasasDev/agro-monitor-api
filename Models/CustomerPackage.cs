using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroMonitor.Models
{
    public class CustomerPackage
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        [ForeignKey(nameof(Package))]
        public int PackageId { get; set; }
        public Package Package { get; set; } = null!;

        public DateTime AssignedAt { get; set; }
    }
}
