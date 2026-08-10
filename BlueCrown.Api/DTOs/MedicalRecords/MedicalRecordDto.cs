using System;

namespace BlueCrown.Api.DTOs.MedicalRecords
{
    public class MedicalRecordDto
    {
        public Guid Id { get; set; }

        public Guid? AppointmentId { get; set; }

        public Guid PatientId { get; set; }

        public Guid DoctorId { get; set; }

        public string Diagnosis { get; set; } = null!;

        public string? Notes { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}