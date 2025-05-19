using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroMonitor.Models
{
    public class Device
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string DeviceUniqueIdentifier { get; set; } = null!;
        [Required]
        public string Name { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public List<SensorReading> Readings { get; set; } = new List<SensorReading>();
    }
}
