using BlueCrown.Api.DTOs.Users;
namespace BlueCrown.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        Task<UserDetailDto?> GetUserByIdAsync(Guid id);

        Task<bool> RegisterAsync(RegisterDto dto);

        Task<bool> UpdateUserAsync(Guid id, UpdateUserDto dto);

        Task<bool> DeleteUserAsync(Guid id);
    }
}
