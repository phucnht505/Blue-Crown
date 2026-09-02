namespace BlueCrown.Api.DTOs.Prescriptions
{
    public class PrescriptionDto
    {
        public Guid Id { get; set; }

        public Guid AppointmentId { get; set; }

        public Guid? MedicalRecordId { get; set; }

        public string MedicalRecordDiagnosis { get; set; } = string.Empty;

        public string Diagnosis { get; set; } = string.Empty;

        public DateTime? AppointmentScheduledAt { get; set; }

        public string? AppointmentType { get; set; }

        public Guid PatientId { get; set; }

        public string PatientName { get; set; } = string.Empty;

        public Guid DoctorId { get; set; }

        public string DoctorName { get; set; } = string.Empty;

        public string DoctorSpecialty { get; set; } = string.Empty;

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public List<PrescriptionItemDto> Items { get; set; } = new();
    }
}