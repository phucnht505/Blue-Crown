using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.PatientProfiles
{
    public class CreatePatientProfileDto
    {
        [Required]
        public string BloodType { get; set; } = string.Empty;

        [Required]
        public decimal HeightCm { get; set; }

        [Required]
        public decimal WeightKg { get; set; }

        public string? Allergies { get; set; }

        public string? ChronicConditions { get; set; }

        public string? EmergencyContactName { get; set; }

        public string? EmergencyContactPhone { get; set; }
    }
}