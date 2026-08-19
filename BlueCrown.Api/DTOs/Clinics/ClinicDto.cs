namespace BlueCrown.Api.DTOs.Clinics
{
    public class ClinicDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public string? Phone { get; set; }
    }
}