using BlueCrown.Api.DTOs.Users;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IAdminPharmacistService
    {
        Task<IEnumerable<UserDto>> GetAllAsync(string? search = null, string? status = null);
        Task<UserDetailDto?> GetByIdAsync(Guid id);
        Task<UserDetailDto> CreateAsync(AdminPharmacistCreateDto dto);
        Task<UserDetailDto> UpdateAsync(Guid id, AdminPharmacistUpdateDto dto, Guid currentAdminId);
        Task<UserDetailDto> UpdateStatusAsync(Guid id, UpdateUserStatusDto dto, Guid currentAdminId);
        Task<string> DeleteAsync(Guid id, Guid currentAdminId);
    }
}