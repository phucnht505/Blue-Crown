using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.Appointments
{
    public class UpdateAppointmentStatusDto
    {
        [Required(ErrorMessage = "Vui lòng chọn trạng thái lịch khám.")]
        [RegularExpression("^(confirmed|cancelled|completed)$", ErrorMessage = "Trạng thái lịch khám không hợp lệ.")]
        public string Status { get; set; } = string.Empty;
    }
}