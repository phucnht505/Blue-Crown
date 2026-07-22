using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class PatientProfile
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? BloodType { get; set; }

    public decimal? HeightCm { get; set; }

    public string? Allergies { get; set; }

    public string? ChronicConditions { get; set; }

    public string? EmergencyContactName { get; set; }

    public string? EmergencyContactPhone { get; set; }

    public decimal? WeightKg { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();

    public virtual ICollection<HealthGoal> HealthGoals { get; set; } = new List<HealthGoal>();

    public virtual ICollection<HealthMetric> HealthMetrics { get; set; } = new List<HealthMetric>();

    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

    public virtual ICollection<SymptomLog> SymptomLogs { get; set; } = new List<SymptomLog>();

    public virtual User User { get; set; } = null!;
}
