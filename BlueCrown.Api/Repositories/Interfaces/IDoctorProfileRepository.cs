using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IDoctorProfileRepository
    {
        Task<List<DoctorProfile>> GetAllAsync();

        Task<DoctorProfile?> GetByIdAsync(Guid id);

        Task<DoctorProfile?> GetByUserIdAsync(Guid userId);

        Task AddAsync(DoctorProfile doctorProfile);

        Task UpdateAsync(DoctorProfile doctorProfile);

        Task SaveChangesAsync();
    }
}