namespace BlueCrown.Api.DTOs.Prescriptions
{
    public class PrescriptionCheckoutOptionDto
    {
        public Guid Id { get; set; }
        public string? AppointmentType { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string MedicalRecordDiagnosis { get; set; } = string.Empty;
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<PrescriptionCheckoutItemDto> Items { get; set; } = new();
    }

    public class PrescriptionCheckoutItemDto
    {
        public Guid MedicationId { get; set; }
    }
}