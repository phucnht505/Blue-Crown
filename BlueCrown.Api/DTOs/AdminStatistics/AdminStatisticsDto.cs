using System;
using System.Collections.Generic;

namespace BlueCrown.Api.DTOs.AdminStatistics;

public class PrescriptionDispenseStatistic
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int QuantityDispensed { get; set; }
    public decimal UnitPrice { get; set; }
    public string? DispensedByName { get; set; }
    public DateTime? DispensedAt { get; set; }
    public decimal TotalAmount { get; set; }
}

public class AdminStatisticsDto
{
    public string Period { get; set; } = string.Empty;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }

    public int SalesOrderCount { get; set; }
    public decimal SalesRevenue { get; set; }

    public int DispensedItemCount { get; set; }
    public decimal PrescriptionRevenue { get; set; }
    public List<PrescriptionDispenseStatistic> DispensedItems { get; set; } = [];

    public decimal TotalRevenue { get; set; }

    public int InventoryReceiptCount { get; set; }
    public decimal InventoryCost { get; set; }

    public decimal Balance { get; set; }

    public List<SalesOrderStatistic> SalesOrders { get; set; } = [];
    public List<InventoryReceiptStatistic> InventoryReceipts { get; set; } = [];
}

public class SalesOrderStatistic
{
    public Guid Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? GuestPhone { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string OrderStatus { get; set; } = string.Empty;
}

public class InventoryReceiptStatistic
{
    public Guid Id { get; set; }
    public DateTime? ReceiptDate { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public decimal TotalCost { get; set; }
    public string Status { get; set; } = string.Empty;
}