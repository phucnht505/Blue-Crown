using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IPrescriptionRepository
    {
        Task<List<Prescription>> GetAllAsync();

        Task<Prescription?> GetByIdAsync(Guid id);

        Task<Prescription> AddAsync(Prescription prescription);

        Task UpdateAsync(Prescription prescription);

        Task DeleteAsync(Prescription prescription);

        Task SaveChangesAsync();
    }
}