namespace AgroMonitor.DTOs
{
    public class DeviceDTO
    {
        public int Id { get; set; }
        public string DeviceUniqueIdentifier { get; set; } = null!;
        public string Name { get; set; } = null!;
        public DateTime RegistrationDate { get; set; }
        public List<SensorReadingDTO> Readings { get; set; } = new List<SensorReadingDTO>();
    }
}
