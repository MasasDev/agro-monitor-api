using System.ComponentModel.DataAnnotations;

namespace AgroMonitor.DTOs
{
    public class AssignPackageToCustomerDTO
    {
        [Required]
        [Range(1, long.MaxValue)]
        public int PackageId { get; set; }

        [Required]
        [Range(1, long.MaxValue)]
        public int CustomerId { get; set; }
    }
}
