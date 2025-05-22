namespace AgroMonitor.DTOs
{
    public class SensorReadingDTO()
    {
        public int Id { get; set; }
        public string SensorType { get; set; } = null!;
        public double SensorValue { get; set; }
        public DateTime Timestamp { get; set; }
        public string DeviceName { get; set; } = null!;
        public SensorReadingBatchDTO? Batch { get; set; }

    }
}
