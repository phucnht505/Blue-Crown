using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.Prescriptions
{
    public class CreatePrescriptionItemDto
    {
        public Guid MedicationId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập liều dùng.")]
        [StringLength(100, ErrorMessage = "Liều dùng không được vượt quá 100 ký tự.")]
        public string Dosage { get; set; } = string.Empty;

        [Range(1, 20, ErrorMessage = "Số lần dùng mỗi ngày phải lớn hơn 0.")]
        public int? FrequencyPerDay { get; set; }

        [Range(1, 365, ErrorMessage = "Số ngày sử dụng phải lớn hơn 0.")]
        public int? DurationDays { get; set; }

        [StringLength(500, ErrorMessage = "Hướng dẫn sử dụng không được vượt quá 500 ký tự.")]
        public string? Instructions { get; set; }
    }
}