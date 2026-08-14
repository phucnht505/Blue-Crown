using BlueCrown.Api.DTOs.DoctorProfiles;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IDoctorProfileService
    {
        Task<List<DoctorProfileDto>> GetAllAsync();

        Task<DoctorProfileDto?> GetByIdAsync(Guid id);

        Task<DoctorProfileDto?> GetByUserIdAsync(Guid userId);

        Task<DoctorProfileDto> CreateAsync(
            Guid userId,
            CreateDoctorProfileDto dto);

        Task<bool> UpdateAsync(
            Guid id,
            UpdateDoctorProfileDto dto);

        Task<bool> VerifyLicenseAsync(
            Guid id,
            bool verified);
    }
}