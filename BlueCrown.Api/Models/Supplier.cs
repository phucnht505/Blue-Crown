using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class Supplier
{
    public Guid Id { get; set; }

    public string SupplierName { get; set; } = null!;

    public string? ContactPhone { get; set; }

    public bool? GdpCertified { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<InventoryReceipt> InventoryReceipts { get; set; } = new List<InventoryReceipt>();
}
