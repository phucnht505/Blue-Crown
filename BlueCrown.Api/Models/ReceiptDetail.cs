using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class ReceiptDetail
{
    public Guid Id { get; set; }

    public Guid? ReceiptId { get; set; }

    public Guid? ProductId { get; set; }

    public string BatchNumber { get; set; } = null!;

    public DateOnly ExpirationDate { get; set; }

    public int QuantityImported { get; set; }

    public decimal ImportPrice { get; set; }

    public virtual Product? Product { get; set; }

    public virtual InventoryReceipt? Receipt { get; set; }
}
