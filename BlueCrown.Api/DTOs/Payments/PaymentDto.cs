namespace BlueCrown.Api.DTOs.Payments
{
    public class PaymentDto
    {
        public Guid Id { get; set; }

        public Guid AppointmentId { get; set; }

        public Guid PatientId { get; set; }

        public decimal Amount { get; set; }

        public decimal? PlatformFee { get; set; }

        public string? Status { get; set; }

        public string? PaymentMethod { get; set; }

        public string? TransactionRef { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}