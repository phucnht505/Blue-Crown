using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.DoctorProfiles
{
    public class UpdateDoctorStatusDto
    {
        [Required]
        [RegularExpression(@"^(active|suspended|pending)$", ErrorMessage = "Trạng thái không hợp lệ.")]
        public string Status { get; set; } = string.Empty;
    }
}