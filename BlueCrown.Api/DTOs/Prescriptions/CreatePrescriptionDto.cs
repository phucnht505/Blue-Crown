using System;
using System.Collections.Generic;

namespace BlueCrown.Api.DTOs.Prescriptions
{
    public class CreatePrescriptionDto
    {
        public Guid MedicalRecordId { get; set; }

        public Guid PatientId { get; set; }

        public Guid DoctorId { get; set; }

        public string? Status { get; set; }

        public List<CreatePrescriptionItemDto> Items { get; set; } = new();
    }
}