using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IDoctorProfileRepository
    {
        Task<List<DoctorProfile>> GetAllAsync(string? search = null, string? specialty = null, string? status = null);
        Task<List<DoctorProfile>> GetBookableAsync();
        Task<DoctorProfile?> GetByIdAsync(Guid id);
        Task<DoctorProfile?> GetByUserIdAsync(Guid userId);
        Task<DoctorProfile?> GetByLicenseNumberAsync(string licenseNumber);
        Task<bool> ClinicExistsAsync(Guid clinicId);
        Task<List<Clinic>> GetClinicsAsync();
        Task<List<string>> GetSpecialtiesAsync();
        Task AddAsync(DoctorProfile doctorProfile);
        Task UpdateAsync(DoctorProfile doctorProfile);
        Task SaveChangesAsync();
    }
}