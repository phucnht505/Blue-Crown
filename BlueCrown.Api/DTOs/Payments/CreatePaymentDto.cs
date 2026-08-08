namespace BlueCrown.Api.DTOs.Payments
{
    public class CreatePaymentDto
    {
        public Guid AppointmentId { get; set; }

        public Guid PatientId { get; set; }

        public decimal Amount { get; set; }

        public decimal? PlatformFee { get; set; }

        public string? PaymentMethod { get; set; }

        public string? TransactionRef { get; set; }
    }
}