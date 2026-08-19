namespace BlueCrown.Api.DTOs.Clinics
{
    public class CreateClinicDto
    {
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public string? Phone { get; set; }
    }
}