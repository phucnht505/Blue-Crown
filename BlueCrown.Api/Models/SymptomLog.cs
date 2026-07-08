using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class SymptomLog
{
    public Guid Id { get; set; }

    public Guid? PatientId { get; set; }

    public string SymptomsDescription { get; set; } = null!;

    public string? PredictedDisease { get; set; }

    public string? SeverityLevel { get; set; }

    public string? AiAdvice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual PatientProfile? Patient { get; set; }
}
