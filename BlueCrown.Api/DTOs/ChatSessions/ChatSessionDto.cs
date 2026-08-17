using System;

namespace BlueCrown.Api.DTOs.ChatSessions
{
    public class ChatSessionDto
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }

        public Guid? DoctorId { get; set; }

        public Guid? AiSymptomLogId { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}