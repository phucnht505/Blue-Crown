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

    public string? ActiveIngredient { get; set; }

    public string? TherapeuticGroup { get; set; }

    public string? DosageForm { get; set; }

    public string? Strength { get; set; }

    public bool PrescriptionRequired { get; set; }

    public virtual ICollection<AutoPrescription> AutoPrescriptions { get; set; } = new List<AutoPrescription>();

    public virtual ICollection<DrugAlternative> DrugAlternativeAlternativeProducts { get; set; } = new List<DrugAlternative>();

    public virtual ICollection<DrugAlternative> DrugAlternativeProducts { get; set; } = new List<DrugAlternative>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<ReceiptDetail> ReceiptDetails { get; set; } = new List<ReceiptDetail>();
}
