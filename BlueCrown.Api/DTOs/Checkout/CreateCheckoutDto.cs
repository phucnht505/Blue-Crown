using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.Checkout
{
    public class CreateCheckoutDto
    {
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng.")]
        public string ShippingAddress { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán.")]
        public string PaymentMethod { get; set; } = null!;

        public Guid? PrescriptionId { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string? GuestPhone { get; set; }

        [MinLength(1, ErrorMessage = "Giỏ hàng không được để trống.")]
        public List<CreateCheckoutItemDto> Items { get; set; }
            = new List<CreateCheckoutItemDto>();
    }
}