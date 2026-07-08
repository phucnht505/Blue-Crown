using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class Payment
{
    public Guid Id { get; set; }

    public Guid AppointmentId { get; set; }

    public Guid PatientId { get; set; }

    public decimal Amount { get; set; }

    public decimal? PlatformFee { get; set; }

    public string? Status { get; set; }

    public string? PaymentMethod { get; set; }

    public string? TransactionRef { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual PatientProfile Patient { get; set; } = null!;
}
