namespace BlueCrown.Api.DTOs.Appointments
{
    public class AppointmentDoctorDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public string? ClinicName { get; set; }
        public int? YearsExperience { get; set; }
        public decimal? ConsultationFee { get; set; }
        public decimal? RatingAvg { get; set; }
    }
}