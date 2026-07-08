using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class MedicalRecord
{
    public Guid Id { get; set; }

    public Guid? AppointmentId { get; set; }

    public Guid PatientId { get; set; }

    public Guid DoctorId { get; set; }

    public string Diagnosis { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Appointment? Appointment { get; set; }

    public virtual DoctorProfile Doctor { get; set; } = null!;

    public virtual PatientProfile Patient { get; set; } = null!;

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
