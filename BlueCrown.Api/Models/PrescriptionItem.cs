using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class PrescriptionItem
{
    public Guid Id { get; set; }

    public Guid PrescriptionId { get; set; }

    public Guid MedicationId { get; set; }

    public string? Dosage { get; set; }

    public int? FrequencyPerDay { get; set; }

    public int? DurationDays { get; set; }

    public string? Instructions { get; set; }

    public virtual Medication Medication { get; set; } = null!;

    public virtual Prescription Prescription { get; set; } = null!;
}
