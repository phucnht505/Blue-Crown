using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class ChatSession
{
    public Guid Id { get; set; }

    public Guid PatientId { get; set; }

    public Guid? DoctorId { get; set; }

    public Guid? AiSymptomLogId { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    public virtual DoctorProfile? Doctor { get; set; }

    public virtual PatientProfile Patient { get; set; } = null!;
}
