using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class EcommerceOrder
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public string? GuestPhone { get; set; }

    public string ShippingAddress { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string? PaymentStatus { get; set; }

    public string? OrderStatus { get; set; }

    public Guid? PrescriptionId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Prescription? Prescription { get; set; }

    public virtual User? User { get; set; }
}
