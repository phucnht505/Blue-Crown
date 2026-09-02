using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IMedicationRepository
    {
        Task<List<Medication>> GetAllAsync();
        Task<Medication?> GetByIdAsync(Guid id);
        Task<Medication?> GetByIdForUpdateAsync(Guid id);
        Task<Medication?> GetByNameAsync(string name);
        Task<List<Medication>> GetByIdsAsync(List<Guid> ids);
        Task<bool> HasUsageAsync(Guid id);
        Task AddAsync(Medication medication);
        Task UpdateAsync(Medication medication);
        Task DeleteAsync(Medication medication);
        Task SaveChangesAsync();
    }
}