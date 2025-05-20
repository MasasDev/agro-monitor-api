namespace AgroMonitor.DTOs
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        public string CustomerUniqueIdentifier { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Email { get; set; }
        public string FarmLocation { get; set; } = null!;
        public bool AreRentedDevicesReturned { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? RentedDevicesReturnDate { get; set; }
        public List<PackageDTO> Packages { get; set; } = new();
    }
}
