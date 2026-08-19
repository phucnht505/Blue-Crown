using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IAutoPrescriptionRepository
    {
        Task<List<AutoPrescription>> GetAllAsync();
        Task<AutoPrescription?> GetByIdAsync(Guid id);
        Task<AutoPrescription?> GetByDiseaseNameAsync(string diseaseName);
        Task AddAsync(AutoPrescription autoPrescription);
        Task UpdateAsync(AutoPrescription autoPrescription);
        Task DeleteAsync(AutoPrescription autoPrescription);
        Task SaveChangesAsync();
    }
}