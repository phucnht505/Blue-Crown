using BlueCrown.Api.DTOs.InventoryReceipts;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IFefoService
    {
        Task<List<ReceiptDetailDto>> GetFefoAsync(Guid productId);
    }
}