using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.PatientProfiles
{
    public class UpdatePatientProfileDto
    {
        [Required(ErrorMessage = "Vui lòng chọn nhóm máu.")]
        [RegularExpression(@"^(A|B|AB|O)[+-]$", ErrorMessage = "Nhóm máu không hợp lệ.")]
        public string BloodType { get; set; } = string.Empty;

        [Range(50, 250, ErrorMessage = "Chiều cao phải từ 50 đến 250 cm.")]
        public decimal HeightCm { get; set; }

        [Range(2, 300, ErrorMessage = "Cân nặng phải từ 2 đến 300 kg.")]
        public decimal WeightKg { get; set; }

        [StringLength(500, ErrorMessage = "Thông tin dị ứng không được vượt quá 500 ký tự.")]
        public string? Allergies { get; set; }

        [StringLength(500, ErrorMessage = "Thông tin bệnh nền không được vượt quá 500 ký tự.")]
        public string? ChronicConditions { get; set; }

        [StringLength(100, ErrorMessage = "Tên người liên hệ không được vượt quá 100 ký tự.")]
        public string? EmergencyContactName { get; set; }

        [RegularExpression(@"^(03|05|07|08|09)\d{8}$", ErrorMessage = "Số điện thoại liên hệ khẩn cấp không hợp lệ.")]
        public string? EmergencyContactPhone { get; set; }
    }
}