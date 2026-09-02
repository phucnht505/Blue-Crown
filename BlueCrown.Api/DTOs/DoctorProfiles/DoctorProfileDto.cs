namespace BlueCrown.Api.DTOs.DoctorProfiles
{
    public class DoctorProfileDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? AvatarUrl { get; set; }
        public string? UserStatus { get; set; }
        public string Specialty { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public bool? LicenseVerified { get; set; }
        public string? Bio { get; set; }
        public int? YearsExperience { get; set; }
        public Guid? ClinicId { get; set; }
        public string? ClinicName { get; set; }
        public string? ClinicAddress { get; set; }
        public string? ClinicPhone { get; set; }
        public decimal? ConsultationFee { get; set; }
        public decimal? RatingAvg { get; set; }
        public int? RatingCount { get; set; }
    }
}