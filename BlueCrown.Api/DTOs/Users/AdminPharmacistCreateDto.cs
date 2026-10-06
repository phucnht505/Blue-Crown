using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.Users
{
    public class AdminPharmacistCreateDto
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Họ tên phải từ 2 đến 50 ký tự.")]
        [RegularExpression(@"^(DS\.\s*)?[\p{L}\s]+$", ErrorMessage = "Họ tên chỉ được chứa chữ cái, khoảng trắng và tiền tố DS.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [RegularExpression(@"^(03|05|07|08|09)\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Mật khẩu phải có chữ hoa, chữ thường và số.")]
        public string Password { get; set; } = string.Empty;

        public DateOnly? DateOfBirth { get; set; }

        [RegularExpression(@"^(male|female|other)$", ErrorMessage = "Giới tính không hợp lệ.")]
        public string? Gender { get; set; }

        [RegularExpression(@"^(active|suspended|pending)$", ErrorMessage = "Trạng thái tài khoản không hợp lệ.")]
        public string Status { get; set; } = "active";
    }
}