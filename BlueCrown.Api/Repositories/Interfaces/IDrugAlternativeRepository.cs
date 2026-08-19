using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IDrugAlternativeRepository
    {
        Task<List<DrugAlternative>> GetAllAsync();
        Task<DrugAlternative?> GetByIdAsync(Guid id);
        Task<List<DrugAlternative>> GetByProductIdAsync(Guid productId);
        Task<bool> ProductExistsAsync(Guid productId);
        Task<bool> ExistsAsync(Guid productId, Guid alternativeProductId, Guid? excludeId = null);
        Task AddAsync(DrugAlternative drugAlternative);
        Task UpdateAsync(DrugAlternative drugAlternative);
        Task DeleteAsync(DrugAlternative drugAlternative);
        Task SaveChangesAsync();
    }
}