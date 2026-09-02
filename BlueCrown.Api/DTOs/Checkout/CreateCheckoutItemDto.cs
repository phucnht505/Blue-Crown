using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.Checkout
{
    public class CreateCheckoutItemDto
    {
        [Required(ErrorMessage = "Mã sản phẩm là bắt buộc.")]
        public Guid ProductId { get; set; }

        [Range(1, 999, ErrorMessage = "Số lượng sản phẩm phải từ 1 đến 999.")]
        public int Quantity { get; set; }
    }
}