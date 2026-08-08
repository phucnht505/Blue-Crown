namespace BlueCrown.Api.DTOs.Checkout
{
    public class CreateCheckoutItemDto
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
}