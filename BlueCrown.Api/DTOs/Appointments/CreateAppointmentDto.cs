using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.Appointments
{
    public class CreateAppointmentDto
    {
        public Guid DoctorId { get; set; }

        public DateTime ScheduledAt { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hình thức khám.")]
        [RegularExpression("^(online_consult|clinic_visit)$", ErrorMessage = "Hình thức khám không hợp lệ.")]
        public string Type { get; set; } = string.Empty;
    }
}