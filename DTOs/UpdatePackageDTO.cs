namespace AgroMonitor.DTOs
{
    public class UpdatePackageDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int DeviceId { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}
