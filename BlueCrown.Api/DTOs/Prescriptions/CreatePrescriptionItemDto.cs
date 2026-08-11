using System;

namespace BlueCrown.Api.DTOs.Prescriptions
{
    public class CreatePrescriptionItemDto
    {
        public Guid MedicationId { get; set; }

        public string? Dosage { get; set; }

        public int? FrequencyPerDay { get; set; }

        public int? DurationDays { get; set; }

        public string? Instructions { get; set; }
    }
}