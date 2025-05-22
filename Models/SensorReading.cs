using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroMonitor.Models
{
    public class SensorReading
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string SensorType { get; set; } = string.Empty;
        [Required]
        public double SensorValue { get; set; }
        public DateTime TimeStamp { get; set; }

        [ForeignKey("Device")]
        public int DeviceId { get; set; }
        public Device Device { get; set; } = null!;

        [ForeignKey(nameof(Batch))]
        public int? BatchId { get; set; }
        public SensorReadingBatch? Batch { get; set; }
    }
}
