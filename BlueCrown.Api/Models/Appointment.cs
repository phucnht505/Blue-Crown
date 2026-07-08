using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class Appointment
{
    public Guid Id { get; set; }

    public Guid? ChatSessionId { get; set; }

    public Guid PatientId { get; set; }

    public Guid DoctorId { get; set; }

    public DateTime ScheduledAt { get; set; }

    public string? Type { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ChatSession? ChatSession { get; set; }

    public virtual DoctorProfile Doctor { get; set; } = null!;

    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    public virtual PatientProfile Patient { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
