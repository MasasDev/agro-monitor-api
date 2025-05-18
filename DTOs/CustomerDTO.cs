using AgroMonitor.Models;
using System.ComponentModel.DataAnnotations;

namespace AgroMonitor.DTOs
{
    public class CustomerDTO
    {
        public string CustomerUniqueIdentifier { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Email { get; set; }
        public string FarmLocation { get; set; } = null!;
        public bool IsEquipmentReturned { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? EquipmentReturnDate { get; set; }
        public List<Device> Equipment { get; set; } = new List<Device>();
    }
}
