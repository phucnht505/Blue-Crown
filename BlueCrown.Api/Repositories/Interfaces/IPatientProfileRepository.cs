using BlueCrown.Api.Models;
namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IPatientProfileRepository
    {
        Task<PatientProfile?> GetByUserIdAsync(Guid userId);
        Task AddAsync(PatientProfile patientProfile);
        Task UpdateAsync(PatientProfile patientProfile);
        Task SaveChangesAsync();
    }
}
