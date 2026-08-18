using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface ISymptomLogRepository
    {
        Task<List<SymptomLog>> GetByPatientIdAsync(Guid patientId);

        Task<SymptomLog?> GetByIdAsync(Guid id);

        Task<SymptomLog?> GetLatestByPatientIdAsync(Guid patientId);

        Task AddAsync(SymptomLog symptomLog);

        Task UpdateAsync(SymptomLog symptomLog);

        Task SaveChangesAsync();
    }
}