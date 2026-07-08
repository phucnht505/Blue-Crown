using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.Users
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Họ tên không được để trống.")]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [RegularExpression(@"^(0|\+84)[3|5|7|8|9][0-9]{8}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string? Phone { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;

        public DateOnly? DateOfBirth { get; set; }

        public string? Gender { get; set; }
    }
}
