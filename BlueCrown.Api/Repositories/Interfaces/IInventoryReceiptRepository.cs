using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IInventoryReceiptRepository
    {
        Task<IEnumerable<InventoryReceipt>> GetAllAsync();
        Task<InventoryReceipt?> GetByIdAsync(Guid id);
        Task AddAsync(InventoryReceipt receipt);
        Task SaveChangesAsync();
    }
}