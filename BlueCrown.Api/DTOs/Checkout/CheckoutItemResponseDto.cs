namespace BlueCrown.Api.DTOs.Checkout
{
    public class CheckoutItemResponseDto
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }
    }
}