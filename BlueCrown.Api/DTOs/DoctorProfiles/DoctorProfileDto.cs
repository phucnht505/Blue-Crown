using System;

namespace BlueCrown.Api.DTOs.DoctorProfiles
{
    public class DoctorProfileDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Specialty { get; set; } = null!;

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