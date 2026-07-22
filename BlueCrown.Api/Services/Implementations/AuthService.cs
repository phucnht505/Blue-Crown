using BCrypt.Net;
using BlueCrown.Api.DTOs.Users;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public AuthService(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email!);
            if (user == null)  
                throw new Exception("Email không tồn tại!");
            bool checkPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!checkPassword)
                throw new Exception("Mật khẩu không chính xác!");
            string token = _jwtService.GenerateToken(user);

            return new LoginResponseDto
            {
                UserID = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                Token = token,
                ExpiresAt = DateTime.Now.AddHours(12)
            };
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            var email = await _userRepository.GetByEmailAsync(dto.Email);
            if (email != null)
                throw new Exception("Email đã tồn tại!");
            var phone = await _userRepository.GetByPhoneAsync(dto.Phone);
            if(phone != null)
                throw new Exception("Số điện thoại đã tồn tại!");
            if (dto.DateOfBirth.HasValue)
            {
                int age = DateTime.Now.Year - dto.DateOfBirth.Value.Year;
                if (age < 16)
                    throw new Exception("Người dùng phải từ 16 tuổi trở lên");
            }
            if (!string.IsNullOrWhiteSpace(dto.Gender))
            {
                string gender = dto.Gender.Trim().ToLower();
                if (gender != "male" && gender != "female" && gender != "other")
                    throw new Exception("Giới tính không hợp lệ!");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            User user = new User
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = passwordHash,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                AvatarUrl = null,
                Role = "Patient",
                Status = "Active",
                EmailVerifiedAt = null,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            return "Đăng ký thành công.";
        }
    }
}