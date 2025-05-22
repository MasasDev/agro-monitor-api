
namespace AgroMonitor.DTOs
{
    public class SensorReadingBatchDTO
    {
        public int Id { get; set; }
        public ICollection<SensorReadingDTO> Readings { get; set; }
        public string? AISuggestion { get; set; }
        public DeviceDTO Device { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
