namespace BlueCrown.Api.DTOs.Checkout
{
    public class CheckoutResponseDto
    {
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }

        public string? GuestPhone { get; set; }

        public string ShippingAddress { get; set; } = null!;

        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; } = null!;

        public string? PaymentStatus { get; set; }

        public string? OrderStatus { get; set; }

        public Guid? PrescriptionId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public List<CheckoutItemResponseDto> Items { get; set; } = new List<CheckoutItemResponseDto>();
    }
}