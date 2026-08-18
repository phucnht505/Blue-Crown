using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IMedicationRepository
    {
        Task<List<Medication>> GetAllAsync();

        Task<Medication?> GetByIdAsync(Guid id);

        Task AddAsync(Medication medication);

        Task UpdateAsync(Medication medication);

        Task SaveChangesAsync();
    }
}