using System;

namespace BlueCrown.Api.DTOs.Appointments
{
    public class CreateAppointmentDto
    {
        public Guid PatientId { get; set; }

        public Guid DoctorId { get; set; }

        public DateTime ScheduledAt { get; set; }

        public string? Type { get; set; }
    }
}