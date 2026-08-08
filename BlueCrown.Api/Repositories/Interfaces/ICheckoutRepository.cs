using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface ICheckoutRepository
    {
        Task AddAsync(EcommerceOrder order);

        Task<EcommerceOrder?> GetByIdAsync(Guid id);

        Task SaveChangesAsync();
    }
}