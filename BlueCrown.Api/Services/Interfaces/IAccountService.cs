using BlueCrown.Api.DTOs.Users;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IAccountService
    {
        Task<UserDetailDto?> GetMyProfileAsync(Guid userId);
        Task<UserDetailDto> UpdateMyProfileAsync(Guid userId, UpdateAccountProfileDto dto);
    }
}