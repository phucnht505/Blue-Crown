using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class InventoryReceipt
{
    public Guid Id { get; set; }

    public Guid? SupplierId { get; set; }

    public Guid? CreatedBy { get; set; }

    public Guid? ApprovedBy { get; set; }

    public decimal? TotalCost { get; set; }

    public DateTime? ReceiptDate { get; set; }

    public string? Status { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<ReceiptDetail> ReceiptDetails { get; set; } = new List<ReceiptDetail>();

    public virtual Supplier? Supplier { get; set; }
}
