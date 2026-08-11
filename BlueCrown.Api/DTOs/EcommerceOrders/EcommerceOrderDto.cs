using System;
using System.Collections.Generic;

namespace BlueCrown.Api.DTOs.EcommerceOrders
{
    public class EcommerceOrderDto
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

        public List<OrderItemDto> Items { get; set; } = new();
    }
}