using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class HealthGoal
{
    public Guid Id { get; set; }

    public Guid PatientId { get; set; }

    public int MetricTypeId { get; set; }

    public decimal? TargetValue { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Status { get; set; }

    public virtual MetricType MetricType { get; set; } = null!;

    public virtual PatientProfile Patient { get; set; } = null!;
}
