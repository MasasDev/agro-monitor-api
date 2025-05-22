using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroMonitor.Models
{
    public class SensorReadingBatch
    {
        [Key]
        public int Id { get; set; }

        public ICollection<SensorReading> SensorReadings { get; set;}

        public string? AISuggestion { get; set; }


        [ForeignKey(nameof(Device))]
        public int DeviceId { get; set; }
        public Device Device { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
