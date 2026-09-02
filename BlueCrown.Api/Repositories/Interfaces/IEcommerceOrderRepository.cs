using BlueCrown.Api.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IEcommerceOrderRepository
    {
        Task<List<EcommerceOrder>> GetAllAsync();
        Task<EcommerceOrder?> GetByIdAsync(Guid id);
        Task<EcommerceOrder?> GetByIdForUpdateAsync(Guid id);
        Task<List<EcommerceOrder>> GetByUserIdAsync(Guid userId);
        Task<List<EcommerceOrder>> GetGuestOrdersByPhoneAsync(string guestPhone);
        Task<bool> HasActiveOrderByPrescriptionIdAsync(Guid prescriptionId);
        Task AddAsync(EcommerceOrder order);
        Task SaveChangesAsync();
        Task<IDbContextTransaction> BeginSerializableTransactionAsync();
    }
}