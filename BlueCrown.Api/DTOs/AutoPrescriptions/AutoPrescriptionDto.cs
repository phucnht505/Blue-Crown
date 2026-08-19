using System;

namespace BlueCrown.Api.DTOs.AutoPrescriptions
{
    public class AutoPrescriptionDto
    {
        public Guid Id { get; set; }
        public string DiseaseName { get; set; } = null!;
        public Guid? RecommendedProductId { get; set; }
        public string? DosageInstructions { get; set; }
    }
}