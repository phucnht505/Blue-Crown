namespace BlueCrown.Api.DTOs.Prescriptions
{
    public class PrescriptionItemDto
    {
        public Guid Id { get; set; }
        public Guid PrescriptionId { get; set; }
        public Guid MedicationId { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public string? GenericName { get; set; }
        public string? Category { get; set; }
        public string? Dosage { get; set; }
        public int? FrequencyPerDay { get; set; }
        public int? DurationDays { get; set; }
        public string? Instructions { get; set; }
        public PrescriptionDispenseItemDto? Dispense { get; set; }
    }
}