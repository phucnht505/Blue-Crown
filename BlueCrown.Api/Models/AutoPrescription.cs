using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class AutoPrescription
{
    public Guid Id { get; set; }

    public string DiseaseName { get; set; } = null!;

    public Guid? RecommendedProductId { get; set; }

    public string? DosageInstructions { get; set; }

    public virtual Product? RecommendedProduct { get; set; }
}
