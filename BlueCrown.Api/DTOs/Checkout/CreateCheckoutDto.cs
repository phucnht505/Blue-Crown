using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.Checkout
{
    public class CreateCheckoutDto
    {
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng.")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Địa chỉ giao hàng phải từ 10 đến 500 ký tự.")]
        public string ShippingAddress { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán.")]
        public string PaymentMethod { get; set; } = null!;

        public Guid? PrescriptionId { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự.")]
        public string? GuestPhone { get; set; }

        [Required(ErrorMessage = "Giỏ hàng không được để trống.")]
        [MinLength(1, ErrorMessage = "Giỏ hàng phải có ít nhất một sản phẩm.")]
        public List<CreateCheckoutItemDto> Items { get; set; } = new();
    }
}