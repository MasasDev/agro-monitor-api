namespace AgroMonitor.DTOs
{
    public class AssignedPackageResponseDTO
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public int PackageId { get; set; }
        public string PackageName { get; set; } = null!;
        public DateTime AssignedAt { get; set; }
    }
}
