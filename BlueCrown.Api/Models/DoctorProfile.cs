using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class DoctorProfile
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Specialty { get; set; } = null!;

    public string LicenseNumber { get; set; } = null!;

    public bool? LicenseVerified { get; set; }

    public string? Bio { get; set; }

    public int? YearsExperience { get; set; }

    public Guid? ClinicId { get; set; }

    public decimal? ConsultationFee { get; set; }

    public decimal? RatingAvg { get; set; }

    public int? RatingCount { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();

    public virtual Clinic? Clinic { get; set; }

    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

    public virtual User User { get; set; } = null!;
}
