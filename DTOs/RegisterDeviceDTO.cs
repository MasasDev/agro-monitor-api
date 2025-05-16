namespace AgroMonitor.DTOs
{
    public class RegisterDeviceDTO()
    {
        public string DeviceIdentifier { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
    }
}
