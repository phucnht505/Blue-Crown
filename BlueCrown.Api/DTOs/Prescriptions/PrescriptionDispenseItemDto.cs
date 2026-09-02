namespace BlueCrown.Api.DTOs.Prescriptions
{
    public class PrescriptionDispenseItemDto
    {
        public Guid Id { get; set; }
        public Guid PrescriptionItemId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int QuantityDispensed { get; set; }
        public Guid? DispensedBy { get; set; }
        public string? DispensedByName { get; set; }
        public DateTime? DispensedAt { get; set; }
    }
}