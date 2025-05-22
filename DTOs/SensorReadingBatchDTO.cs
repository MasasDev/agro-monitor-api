using AgroMonitor.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroMonitor.DTOs
{
    public class SensorReadingBatchDTO
    {
        public int Id { get; set; }
        public ICollection<SensorReading> SensorReadings { get; set; }
        public string? AISuggestion { get; set; }
        public Device Device { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
