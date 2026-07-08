using BlueCrown.Api.Services.Interfaces;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Models;
using BlueCrown.Api.DTOs.Users;
namespace BlueCrown.Api.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
           var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.Phone,
                Role = u.Role,
                Status = u.Status
            });
        }
        public async Task<UserDetailDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return null;
            return new UserDetailDto
            {
                Id = user.Id,
                FullName= user.FullName,
                Email= user.Email,
                Phone= user.Phone,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                AvatarUrl = user.AvatarUrl,
                Role = user.Role,
                Status= user.Status
            };
        }
        // Controller -> Service -> Repository -> Database

        public Task<bool> RegisterAsync(RegisterDto dto)
        {
            throw new NotImplementedException();
        }
        public Task<bool> UpdateUserAsync(Guid id, UpdateUserDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteUserAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
