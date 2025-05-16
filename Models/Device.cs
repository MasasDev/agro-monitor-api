using System.ComponentModel.DataAnnotations;

namespace AgroMonitor.Models
{
    public class Device
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string DeviceIdentifier { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Location { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public List<SensorReading> Readings { get; set; } = new List<SensorReading>();
    }
}
