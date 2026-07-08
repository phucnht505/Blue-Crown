using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? AvatarUrl { get; set; }

    public string Role { get; set; } = null!;

    public string? Status { get; set; }

    public DateTime? EmailVerifiedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    public virtual DoctorProfile? DoctorProfile { get; set; }

    public virtual ICollection<EcommerceOrder> EcommerceOrders { get; set; } = new List<EcommerceOrder>();

    public virtual ICollection<InventoryReceipt> InventoryReceiptApprovedByNavigations { get; set; } = new List<InventoryReceipt>();

    public virtual ICollection<InventoryReceipt> InventoryReceiptCreatedByNavigations { get; set; } = new List<InventoryReceipt>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual PatientProfile? PatientProfile { get; set; }
}
