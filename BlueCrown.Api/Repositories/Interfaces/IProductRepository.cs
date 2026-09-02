using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product?> GetByIdForUpdateAsync(Guid id);
        Task<Product?> GetByNameAsync(string name);
        Task<IEnumerable<Product>> GetByMedicationIdAsync(Guid medicationId);
        Task<List<Product>> GetByIdsForUpdateAsync(List<Guid> ids);
        Task<IEnumerable<Product>> SearchAsync(string keyword);
        Task<bool> HasReferencesAsync(Guid id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task SaveChangesAsync();
    }
}