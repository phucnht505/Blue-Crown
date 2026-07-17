using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class DrugAlternative
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid AlternativeProductId { get; set; }

    public string? Reason { get; set; }

    public decimal? SimilarityScore { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Product AlternativeProduct { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
