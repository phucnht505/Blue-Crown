using BlueCrown.Api.DTOs.AdminStatistics;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations;

public class AdminStatisticsService : IAdminStatisticsService
{
    private readonly IAdminStatisticsRepository _repository;

    public AdminStatisticsService(IAdminStatisticsRepository repository)
    {
        _repository = repository;
    }

    public async Task<AdminStatisticsDto> GetStatisticsAsync(AdminStatisticsQueryDto query)
    {
        var (fromDate, toDate, period) = GetDateRange(query);

        var orders = await _repository.GetDeliveredOrdersAsync(fromDate, toDate);
        var receipts = await _repository.GetApprovedReceiptsAsync(fromDate, toDate);

        var salesRevenue = orders.Sum(x => x.TotalAmount);
        var inventoryCost = receipts.Sum(x => x.TotalCost ?? 0);

        return new AdminStatisticsDto
        {
            Period = period,
            FromDate = fromDate,
            ToDate = toDate,
            SalesOrderCount = orders.Count,
            SalesRevenue = salesRevenue,
            InventoryReceiptCount = receipts.Count,
            InventoryCost = inventoryCost,
            Balance = salesRevenue - inventoryCost,

            SalesOrders = orders.Select(x => new SalesOrderStatisticDto
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                CustomerName = x.User?.FullName ?? "Khách vãng lai",
                GuestPhone = x.GuestPhone,
                TotalAmount = x.TotalAmount,
                PaymentMethod = x.PaymentMethod,
                PaymentStatus = x.PaymentStatus ?? string.Empty,
                OrderStatus = x.OrderStatus ?? string.Empty
            }).ToList(),

            InventoryReceipts = receipts.Select(x => new InventoryReceiptStatisticDto
            {
                Id = x.Id,
                ReceiptDate = x.ReceiptDate,
                SupplierName = x.Supplier?.SupplierName ?? "Không xác định",
                TotalCost = x.TotalCost ?? 0,
                Status = x.Status ?? string.Empty
            }).ToList()
        };
    }

    private static (DateTime fromDate, DateTime toDate, string period) GetDateRange(AdminStatisticsQueryDto query)
    {
        var now = DateTime.Now;
        var period = (query.Period ?? "day").Trim().ToLowerInvariant();

        if (period == "day")
        {
            var date = (query.Date ?? now).Date;
            return (date, date.AddDays(1), "day");
        }

        if (period == "month")
        {
            var year = query.Year ?? now.Year;
            var month = query.Month ?? now.Month;

            if (year < 1 || year > 9999)
            {
                throw new ArgumentException("Năm không hợp lệ.");
            }

            if (month < 1 || month > 12)
            {
                throw new ArgumentException("Tháng phải từ 1 đến 12.");
            }

            var start = new DateTime(year, month, 1);

            return (
                start,
                start.AddMonths(1),
                "month"
            );
        }

        if (period == "year")
        {
            var year = query.Year ?? now.Year;

            if (year < 1 || year > 9999)
            {
                throw new ArgumentException("Năm không hợp lệ.");
            }

            var start = new DateTime(year, 1, 1);

            return (
                start,
                start.AddYears(1),
                "year"
            );
        }

        throw new ArgumentException("Period chỉ nhận day, month hoặc year.");
    }
}