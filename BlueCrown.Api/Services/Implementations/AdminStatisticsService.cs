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
        var (fromDate, toDate) = GetDateRange(
            query.Period,
            query.Date,
            query.Month,
            query.Year);

        var orders = await _repository.GetDeliveredOrdersAsync(fromDate, toDate);
        var receipts = await _repository.GetApprovedReceiptsAsync(fromDate, toDate);
        var dispensedItems = await _repository.GetDispensedItemsAsync(fromDate, toDate);

        var salesRevenue = orders.Sum(x => x.TotalAmount);

        var prescriptionRevenue = dispensedItems.Sum(
            x => x.UnitPrice * x.QuantityDispensed);

        var totalRevenue = salesRevenue + prescriptionRevenue;

        var inventoryCost = receipts.Sum(x => x.TotalCost ?? 0);

        return new AdminStatisticsDto
        {
            Period = query.Period,
            FromDate = fromDate,
            ToDate = toDate,

            SalesOrderCount = orders.Count,
            SalesRevenue = salesRevenue,

            DispensedItemCount = dispensedItems.Count,
            PrescriptionRevenue = prescriptionRevenue,

            DispensedItems = dispensedItems.Select(x => new PrescriptionDispenseStatistic
            {
                Id = x.Id,
                ProductName = x.Product?.Name ?? "Không xác định",
                QuantityDispensed = x.QuantityDispensed,
                UnitPrice = x.UnitPrice,
                DispensedByName = x.DispensedByNavigation?.FullName,
                DispensedAt = x.DispensedAt,
                TotalAmount = x.UnitPrice * x.QuantityDispensed
            }).ToList(),

            TotalRevenue = totalRevenue,

            InventoryReceiptCount = receipts.Count,
            InventoryCost = inventoryCost,

            Balance = totalRevenue - inventoryCost,

            SalesOrders = orders.Select(x => new SalesOrderStatistic
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                CustomerName = GetCustomerName(x),
                GuestPhone = x.GuestPhone,
                TotalAmount = x.TotalAmount,
                PaymentMethod = x.PaymentMethod,
                PaymentStatus = x.PaymentStatus,
                OrderStatus = x.OrderStatus
            }).ToList(),

            InventoryReceipts = receipts.Select(x => new InventoryReceiptStatistic
            {
                Id = x.Id,
                ReceiptDate = x.ReceiptDate,
                SupplierName = x.Supplier?.SupplierName ?? "Không xác định",
                TotalCost = x.TotalCost ?? 0,
                Status = x.Status
            }).ToList()
        };
    }

    private static string GetCustomerName(Models.EcommerceOrder order)
    {
        if (!string.IsNullOrWhiteSpace(order.User?.FullName))
        {
            return order.User.FullName;
        }

        if (!string.IsNullOrWhiteSpace(order.GuestPhone))
        {
            return $"Khách vãng lai {order.GuestPhone}";
        }

        return "Khách vãng lai";
    }

    private static (DateTime FromDate, DateTime ToDate) GetDateRange(
        string period,
        DateTime? date,
        int? month,
        int? year)
    {
        switch (period.ToLowerInvariant())
        {
            case "day":
                {
                    var selectedDate = (date ?? DateTime.Today).Date;

                    return (
                        selectedDate,
                        selectedDate.AddDays(1)
                    );
                }

            case "month":
                {
                    var selectedYear = year ?? DateTime.Today.Year;
                    var selectedMonth = month ?? DateTime.Today.Month;

                    var fromDate = new DateTime(
                        selectedYear,
                        selectedMonth,
                        1);

                    return (
                        fromDate,
                        fromDate.AddMonths(1)
                    );
                }

            case "year":
                {
                    var selectedYear = year ?? DateTime.Today.Year;

                    var fromDate = new DateTime(
                        selectedYear,
                        1,
                        1);

                    return (
                        fromDate,
                        fromDate.AddYears(1)
                    );
                }

            default:
                throw new ArgumentException(
                    "Period không hợp lệ. Chỉ hỗ trợ day, month hoặc year.",
                    nameof(period));
        }
    }
}