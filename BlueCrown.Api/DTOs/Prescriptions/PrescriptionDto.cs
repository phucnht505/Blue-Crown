using System;
using System.Collections.Generic;

namespace BlueCrown.Api.DTOs.Prescriptions
{
    public class PrescriptionDto
    {
        public Guid Id { get; set; }

        public Guid MedicalRecordId { get; set; }

        public Guid PatientId { get; set; }

        public Guid DoctorId { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public List<PrescriptionItemDto> Items { get; set; } = new();
    }
}