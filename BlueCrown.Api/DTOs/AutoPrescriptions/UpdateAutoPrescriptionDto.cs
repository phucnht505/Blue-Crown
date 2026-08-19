using System;

namespace BlueCrown.Api.DTOs.AutoPrescriptions
{
    public class UpdateAutoPrescriptionDto
    {
        public string DiseaseName { get; set; } = null!;
        public Guid? RecommendedProductId { get; set; }
        public string? DosageInstructions { get; set; }
    }
}