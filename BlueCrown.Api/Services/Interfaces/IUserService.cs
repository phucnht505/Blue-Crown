using BlueCrown.Api.DTOs.Users;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync(string? search = null, string? role = null, string? status = null);

        Task<UserDetailDto?> GetUserByIdAsync(Guid id);

        Task<UserDetailDto> CreateUserByAdminAsync(AdminCreateUserDto dto);

        Task<UserDetailDto> UpdateUserByAdminAsync(Guid id, AdminUpdateUserDto dto, Guid currentAdminId);

        Task<UserDetailDto> UpdateUserStatusAsync(Guid id, UpdateUserStatusDto dto, Guid currentAdminId);

        Task<string> DeleteUserByAdminAsync(Guid id, Guid currentAdminId);

        Task<bool> RegisterAsync(RegisterDto dto);

        Task<bool> UpdateUserAsync(Guid id, UpdateUserDto dto);

        Task<bool> DeleteUserAsync(Guid id);
    }
}