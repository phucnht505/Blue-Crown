using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IEcommerceOrderRepository
    {
        Task<List<EcommerceOrder>> GetAllAsync();

        Task<EcommerceOrder?> GetByIdAsync(Guid id);

        Task AddAsync(EcommerceOrder order);

        Task UpdateAsync(EcommerceOrder order);

        Task DeleteAsync(EcommerceOrder order);

        Task<bool> UserExistsAsync(Guid userId);

        Task SaveChangesAsync();
    }
}