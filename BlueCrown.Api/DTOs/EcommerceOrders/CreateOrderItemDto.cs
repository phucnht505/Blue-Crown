namespace BlueCrown.Api.DTOs.EcommerceOrders
{
    public class CreateOrderItemDto
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}