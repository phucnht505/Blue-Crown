using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.Users
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Họ tên không được để trống.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Họ tên phải từ 2 đến 50 ký tự.")]
        [RegularExpression(@"^[\p{L}\s]+$", ErrorMessage = "Họ tên chỉ được chứa chữ cái và khoảng trắng.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [RegularExpression(@"^(03|05|07|08|09)\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Mật khẩu phải có chữ hoa, chữ thường và chữ số.")]
        public string Password { get; set; } = string.Empty;

        public DateOnly? DateOfBirth { get; set; }

        public string? Gender { get; set; }
    }
}