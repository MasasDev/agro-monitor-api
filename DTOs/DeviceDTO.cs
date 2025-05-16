namespace AgroMonitor.DTOs
{
    public class DeviceDTO
    {
        public string DeviceIdentifier { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public DateTime RegistrationDate { get; set; }
        public List<SensorReadingDTO> Readings { get; set; } = new List<SensorReadingDTO>();
    }
}
