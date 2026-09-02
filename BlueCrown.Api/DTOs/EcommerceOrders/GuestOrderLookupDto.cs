using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.EcommerceOrders
{
    public class GuestOrderLookupDto
    {
        public Guid? OrderId { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [StringLength(20, ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string GuestPhone { get; set; } = string.Empty;
    }
}