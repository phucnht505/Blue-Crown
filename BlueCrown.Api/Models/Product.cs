using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int? StockQuantity { get; set; }

    public bool? IsPrescriptionRequired { get; set; }

    public virtual ICollection<AutoPrescription> AutoPrescriptions { get; set; } = new List<AutoPrescription>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<ReceiptDetail> ReceiptDetails { get; set; } = new List<ReceiptDetail>();
}
