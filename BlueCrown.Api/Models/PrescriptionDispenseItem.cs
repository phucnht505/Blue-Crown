using System;

namespace BlueCrown.Api.Models;

public partial class PrescriptionDispenseItem
{
    public Guid Id { get; set; }

    public Guid PrescriptionItemId { get; set; }

    public Guid ProductId { get; set; }

    public int QuantityDispensed { get; set; }

    public Guid? DispensedBy { get; set; }

    public DateTime? DispensedAt { get; set; }

    public virtual PrescriptionItem PrescriptionItem { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual User? DispensedByNavigation { get; set; }
}