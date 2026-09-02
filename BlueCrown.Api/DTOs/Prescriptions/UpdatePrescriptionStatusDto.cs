using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.Prescriptions
{
    public class UpdatePrescriptionStatusDto
    {
        [Required(ErrorMessage = "Vui lòng chọn trạng thái đơn thuốc.")]
        [RegularExpression("^(approved|cancelled)$", ErrorMessage = "Trạng thái đơn thuốc không hợp lệ.")]
        public string Status { get; set; } = string.Empty;
    }
}