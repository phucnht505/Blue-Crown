using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.Users
{
    public class AdminUpdateUserDto
    {
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Họ tên phải từ 2 đến 50 ký tự.")]
        [RegularExpression(@"^[\p{L}\s]+$", ErrorMessage = "Họ tên chỉ được chứa chữ cái và khoảng trắng.")]
        public string? FullName { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string? Email { get; set; }

        [RegularExpression(@"^(03|05|07|08|09)\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string? Phone { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        [RegularExpression(@"^(male|female|other)$", ErrorMessage = "Giới tính không hợp lệ.")]
        public string? Gender { get; set; }

        public string? AvatarUrl { get; set; }

        [RegularExpression(@"^(patient|doctor|pharmacist|admin)$", ErrorMessage = "Vai trò không hợp lệ.")]
        public string? Role { get; set; }

        [RegularExpression(@"^(active|suspended|pending)$", ErrorMessage = "Trạng thái không hợp lệ.")]
        public string? Status { get; set; }
    }
}