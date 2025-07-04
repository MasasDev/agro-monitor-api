﻿namespace AgroMonitor.DTOs
{
    public class UpdateCustomerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Email { get; set; }
        public string FarmLocation { get; set; } = null!;
    }
}
