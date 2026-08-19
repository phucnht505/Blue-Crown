namespace BlueCrown.Api.DTOs.Clinics
{
    public class UpdateClinicDto
    {
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public string? Phone { get; set; }
    }
}