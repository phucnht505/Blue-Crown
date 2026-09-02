namespace BlueCrown.Api.DTOs.Medications
{
    public class MedicationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? GenericName { get; set; }
        public string? Category { get; set; }
    }
}