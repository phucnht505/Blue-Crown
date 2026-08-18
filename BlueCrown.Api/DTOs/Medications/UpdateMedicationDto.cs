namespace BlueCrown.Api.DTOs.Medications
{
    public class UpdateMedicationDto
    {
        public string Name { get; set; } = null!;

        public string? GenericName { get; set; }

        public string? Category { get; set; }
    }
}