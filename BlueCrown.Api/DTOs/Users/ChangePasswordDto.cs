using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.Users
{
    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        //[Required(ErrorMessage = "Mật khẩu không được để trống.")]
        //[MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Mật khẩu phải có chữ hoa, chữ thường và chữ số.")]
        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
}
