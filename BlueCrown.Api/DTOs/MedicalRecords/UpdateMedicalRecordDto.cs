using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.MedicalRecords
{
    public class UpdateMedicalRecordDto
    {
        [Required(ErrorMessage = "Vui lòng nhập chẩn đoán.")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Chẩn đoán phải từ 2 đến 500 ký tự.")]
        public string Diagnosis { get; set; } = string.Empty;

        [StringLength(3000, ErrorMessage = "Ghi chú không được vượt quá 3000 ký tự.")]
        public string? Notes { get; set; }
    }
}