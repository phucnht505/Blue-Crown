namespace BlueCrown.Api.DTOs.Suppliers
{
    public class SupplierDto
    {
        public Guid Id { get; set; }
        public string SupplierName { get; set; } = null!;
        public string? ContactPhone { get; set; }
        public bool? GdpCertified { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}