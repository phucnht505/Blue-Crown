using System;

namespace BlueCrown.Api.DTOs.DoctorProfiles
{
    public class UpdateDoctorProfileDto
    {
        public string Specialty { get; set; } = null!;

        public string? Bio { get; set; }

        public int? YearsExperience { get; set; }

        public Guid? ClinicId { get; set; }

        public decimal? ConsultationFee { get; set; }
    }
}