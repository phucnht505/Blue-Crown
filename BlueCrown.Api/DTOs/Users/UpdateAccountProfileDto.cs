using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.Users
{
    public class UpdateAccountProfileDto
    {
        [Required(ErrorMessage = "Họ tên không được để trống.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Họ tên phải từ 2 đến 50 ký tự.")]
        [RegularExpression(@"^[\p{L}\s]+$", ErrorMessage = "Họ tên chỉ được chứa chữ cái và khoảng trắng.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [RegularExpression(@"^(03|05|07|08|09)\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Phone { get; set; } = string.Empty;

        public DateOnly? DateOfBirth { get; set; }

        [RegularExpression(@"^(male|female|other)$", ErrorMessage = "Giới tính không hợp lệ.")]
        public string? Gender { get; set; }

        [StringLength(1000, ErrorMessage = "Đường dẫn ảnh đại diện không được vượt quá 1000 ký tự.")]
        public string? AvatarUrl { get; set; }
    }
}