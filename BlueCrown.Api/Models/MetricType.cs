using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class MetricType
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public decimal? NormalMin { get; set; }

    public decimal? NormalMax { get; set; }

    public virtual ICollection<HealthGoal> HealthGoals { get; set; } = new List<HealthGoal>();

    public virtual ICollection<HealthMetric> HealthMetrics { get; set; } = new List<HealthMetric>();
}
