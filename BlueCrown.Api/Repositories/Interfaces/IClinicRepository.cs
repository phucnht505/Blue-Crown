using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IClinicRepository
    {
        Task<List<Clinic>> GetAllAsync();
        Task<Clinic?> GetByIdAsync(Guid id);
        Task AddAsync(Clinic clinic);
        Task UpdateAsync(Clinic clinic);
        Task DeleteAsync(Clinic clinic);
        Task<bool> HasDoctorsAsync(Guid clinicId);
        Task SaveChangesAsync();
    }
}