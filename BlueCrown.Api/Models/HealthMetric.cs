using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class HealthMetric
{
    public Guid Id { get; set; }

    public Guid PatientId { get; set; }

    public int MetricTypeId { get; set; }

    public decimal Value { get; set; }

    public DateTime RecordedAt { get; set; }

    public virtual MetricType MetricType { get; set; } = null!;

    public virtual PatientProfile Patient { get; set; } = null!;
}
