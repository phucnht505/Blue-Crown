using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.Clinics
{
    public class CreateClinicDto
    {
        [Required(ErrorMessage = "Tên phòng khám là bắt buộc.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên phòng khám phải từ 2 đến 100 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ phòng khám là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự.")]
        public string Address { get; set; } = string.Empty;
        [RegularExpression(@"^(03|05|07|08|09)\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string? Phone { get; set; }
    }
}