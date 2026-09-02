using BlueCrown.Api.DTOs.InventoryReceipts;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IInventoryReceiptService
    {
        Task<IEnumerable<InventoryReceiptDto>> GetAllAsync();
        Task<InventoryReceiptDto?> GetByIdAsync(Guid id);
        Task CreateAsync(CreateInventoryReceiptDto dto, Guid userId);
        Task<bool> ApproveAsync(Guid receiptId, Guid adminId);
        Task<bool> RejectAsync(Guid receiptId, Guid adminId);
    }
}