using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.Users
{
    public class UpdateUserStatusDto
    {
        [Required(ErrorMessage = "Trạng thái không được để trống.")]
        [RegularExpression(@"^(active|suspended|pending)$", ErrorMessage = "Trạng thái không hợp lệ.")]
        public string Status { get; set; } = string.Empty;
    }
}