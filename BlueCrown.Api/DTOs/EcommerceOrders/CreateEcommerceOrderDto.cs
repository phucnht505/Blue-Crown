namespace BlueCrown.Api.DTOs.EcommerceOrders
{
    public class CreateEcommerceOrderDto
    {
        public Guid? UserId { get; set; }

        public string? GuestPhone { get; set; }

        public string ShippingAddress { get; set; } = null!;

        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; } = null!;

        public Guid? PrescriptionId { get; set; }

        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}