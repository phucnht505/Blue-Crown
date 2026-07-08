using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class Medication
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? GenericName { get; set; }

    public string? Category { get; set; }

    public virtual ICollection<PrescriptionItem> PrescriptionItems { get; set; } = new List<PrescriptionItem>();
}
