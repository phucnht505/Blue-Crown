namespace BlueCrown.Api.DTOs.Appointments
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string DoctorSpecialty { get; set; } = string.Empty;
        public string? ClinicName { get; set; }
        public decimal? ConsultationFee { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}