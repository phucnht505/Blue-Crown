namespace BlueCrown.Api.DTOs.PatientProfiles
{
    public class PatientProfileDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; } = Guid.Empty;
        public string BloodType { get; set; } = string.Empty;
        public decimal? HeightCm { get; set; }
        public decimal? WeightKg { get; set; }
        public string? Allergies { get; set; }
        public string? ChronicConditions { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
    }
}
