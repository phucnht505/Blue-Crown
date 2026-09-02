namespace BlueCrown.Api.DTOs.DoctorProfiles
{
    public class AdminDoctorMetaDto
    {
        public List<string> Specialties { get; set; } = [];
        public List<DoctorClinicOptionDto> Clinics { get; set; } = [];
    }

    public class DoctorClinicOptionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
    }
}