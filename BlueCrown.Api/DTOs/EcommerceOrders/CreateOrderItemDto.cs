using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.EcommerceOrders
{
    public class CreateOrderItemDto
    {
        [Required]
        public Guid ProductId { get; set; }

        [Range(1, 99, ErrorMessage = "Số lượng mỗi Product phải từ 1 đến 99.")]
        public int Quantity { get; set; }
    }
}