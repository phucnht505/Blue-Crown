using System;

namespace BlueCrown.Api.DTOs.DoctorProfiles
{
    public class CreateDoctorProfileDto
    {
        public string Specialty { get; set; } = null!;

        public string LicenseNumber { get; set; } = null!;

        public string? Bio { get; set; }

        public int? YearsExperience { get; set; }

        public Guid? ClinicId { get; set; }

        public decimal? ConsultationFee { get; set; }
    }
}