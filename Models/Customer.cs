using System.ComponentModel.DataAnnotations;

namespace AgroMonitor.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CustomerUniqueIdentifier { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string PhoneNumber { get; set; } = null!;
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string FarmLocation { get; set; } = null!;
        public bool AreRentedDevicesReturned { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? RentedDevicesReturnDate { get; set; }
        public ICollection<CustomerPackage> CustomerPackages { get; set; } = new List<CustomerPackage>();
    }
}
