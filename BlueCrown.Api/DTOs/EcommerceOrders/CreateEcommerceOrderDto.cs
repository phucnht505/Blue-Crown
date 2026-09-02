using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.EcommerceOrders
{
    public class CreateEcommerceOrderDto
    {
        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [StringLength(12, ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string GuestPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ giao hàng không được để trống.")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Địa chỉ giao hàng phải từ 10 đến 500 ký tự.")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phương thức thanh toán không được để trống.")]
        [RegularExpression("^cod$", ErrorMessage = "Hiện tại hệ thống chỉ hỗ trợ thanh toán khi nhận hàng.")]
        public string PaymentMethod { get; set; } = "cod";

        public Guid? PrescriptionId { get; set; }

        [Required(ErrorMessage = "Đơn hàng phải có sản phẩm.")]
        [MinLength(1, ErrorMessage = "Đơn hàng phải có ít nhất một sản phẩm.")]
        [MaxLength(50, ErrorMessage = "Một đơn hàng tối đa 50 loại sản phẩm.")]
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}