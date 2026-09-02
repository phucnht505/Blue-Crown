using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.Prescriptions
{
    public class CreatePrescriptionDto
    {
        public Guid? AppointmentId { get; set; }

        public Guid? MedicalRecordId { get; set; }

        [StringLength(2000, ErrorMessage = "Chẩn đoán không được vượt quá 2000 ký tự.")]
        public string? Diagnosis { get; set; }

        [Required(ErrorMessage = "Đơn thuốc phải có danh sách thuốc.")]
        [MinLength(1, ErrorMessage = "Đơn thuốc phải có ít nhất một loại thuốc.")]
        public List<CreatePrescriptionItemDto> Items { get; set; } = new();
    }
}