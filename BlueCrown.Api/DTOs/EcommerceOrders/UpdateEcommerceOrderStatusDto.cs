using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.EcommerceOrders
{
    public class UpdateEcommerceOrderStatusDto
    {
        [Required(ErrorMessage = "Trạng thái đơn hàng không được để trống.")]
        [RegularExpression("^(confirmed|shipped|delivered|cancelled)$", ErrorMessage = "Trạng thái đơn hàng không hợp lệ.")]
        public string Status { get; set; } = string.Empty;
    }
}