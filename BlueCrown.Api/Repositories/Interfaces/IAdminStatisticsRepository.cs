using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces;

public interface IAdminStatisticsRepository
{
    Task<List<EcommerceOrder>> GetDeliveredOrdersAsync(DateTime fromDate, DateTime toDate);
    Task<List<InventoryReceipt>> GetApprovedReceiptsAsync(DateTime fromDate, DateTime toDate);
}