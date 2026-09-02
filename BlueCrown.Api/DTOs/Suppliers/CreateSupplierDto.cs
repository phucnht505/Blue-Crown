using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.Suppliers
{
    public class CreateSupplierDto
    {
        [Required(ErrorMessage = "Tên nhà cung cấp không được để trống.")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Tên nhà cung cấp phải từ 2 đến 255 ký tự.")]
        public string SupplierName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [RegularExpression(@"^(0[35789]\d{8}|\+84[35789]\d{8})$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string ContactPhone { get; set; } = string.Empty;

        public bool? GdpCertified { get; set; }
    }
}