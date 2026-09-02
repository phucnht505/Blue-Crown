using BlueCrown.Api.DTOs.Users;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto dto);

        Task<LoginResponseDto> LoginAsync(LoginDto dto);

        Task<string> ForgotPasswordAsync(ForgotPasswordDto dto);

        Task<string> ResetPasswordAsync(ResetPasswordDto dto);
    }
}