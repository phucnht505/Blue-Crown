using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations;

public class AdminStatisticsRepository : IAdminStatisticsRepository
{
    private readonly BlueCrownContext _context;

    public AdminStatisticsRepository(BlueCrownContext context)
    {
        _context = context;
    }

    public async Task<List<EcommerceOrder>> GetDeliveredOrdersAsync(DateTime fromDate, DateTime toDate)
    {
        return await _context.EcommerceOrders
            .AsNoTracking()
            .Where(x =>
                x.OrderStatus == "delivered" &&
                x.CreatedAt.HasValue &&
                x.CreatedAt.Value >= fromDate &&
                x.CreatedAt.Value < toDate)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new EcommerceOrder
            {
                Id = x.Id,
                UserId = x.UserId,
                GuestPhone = x.GuestPhone,
                TotalAmount = x.TotalAmount,
                PaymentMethod = x.PaymentMethod,
                PaymentStatus = x.PaymentStatus,
                OrderStatus = x.OrderStatus,
                CreatedAt = x.CreatedAt,
                User = x.User == null
                    ? null
                    : new User
                    {
                        Id = x.User.Id,
                        FullName = x.User.FullName
                    }
            })
            .ToListAsync();
    }

    public async Task<List<InventoryReceipt>> GetApprovedReceiptsAsync(DateTime fromDate, DateTime toDate)
    {
        return await _context.InventoryReceipts
            .AsNoTracking()
            .Where(x =>
                x.Status == "approved" &&
                x.ReceiptDate.HasValue &&
                x.ReceiptDate.Value >= fromDate &&
                x.ReceiptDate.Value < toDate)
            .OrderByDescending(x => x.ReceiptDate)
            .Select(x => new InventoryReceipt
            {
                Id = x.Id,
                SupplierId = x.SupplierId,
                TotalCost = x.TotalCost,
                ReceiptDate = x.ReceiptDate,
                Status = x.Status,
                Supplier = x.Supplier == null
                    ? null
                    : new Supplier
                    {
                        Id = x.Supplier.Id,
                        SupplierName = x.Supplier.SupplierName
                    }
            })
            .ToListAsync();
    }
}