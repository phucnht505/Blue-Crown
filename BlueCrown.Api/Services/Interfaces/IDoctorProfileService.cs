using BlueCrown.Api.DTOs.DoctorProfiles;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IDoctorProfileService
    {
        Task<List<DoctorProfileDto>> GetAllAsync(string? search = null, string? specialty = null, string? status = null);
        Task<DoctorProfileDto?> GetByIdAsync(Guid id);
        Task<DoctorProfileDto?> GetByUserIdAsync(Guid userId);
        Task<DoctorProfileDto> CreateAsync(Guid userId, CreateDoctorProfileDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateDoctorProfileDto dto);
        Task<bool> VerifyLicenseAsync(Guid id, bool verified);
        Task<AdminDoctorMetaDto> GetAdminMetaAsync();
        Task<DoctorProfileDto> AdminCreateAsync(AdminCreateDoctorDto dto);
        Task<DoctorProfileDto> AdminUpdateAsync(Guid id, AdminUpdateDoctorDto dto);
        Task<DoctorProfileDto> AdminUpdateStatusAsync(Guid id, UpdateDoctorStatusDto dto);
        Task<string> AdminDeactivateAsync(Guid id);
    }
}