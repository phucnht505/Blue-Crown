using System;

namespace BlueCrown.Api.DTOs.ChatSessions
{
    public class ChatSessionDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = null!;
        public Guid? DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public Guid? AiSymptomLogId { get; set; }
        public Guid? AppointmentId { get; set; }
        public string? AppointmentType { get; set; }
        public string? AppointmentStatus { get; set; }
        public DateTime? AppointmentScheduledAt { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}