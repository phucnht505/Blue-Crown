namespace BlueCrown.Api.DTOs.Suppliers
{
    public class UpdateSupplierDto
    {
        public string SupplierName { get; set; } = null!;
        public string? ContactPhone { get; set; }
        public bool? GdpCertified { get; set; }
    }
}