using BlueCrown.Api.DTOs.Users;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;

namespace BlueCrown.Api.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _memoryCache;

        private const int OtpExpiryMinutes = 5;
        private const int OtpMaximumAttempts = 5;
        private const int OtpResendSeconds = 60;

        public AuthService(IUserRepository userRepository, IJwtService jwtService, IEmailService emailService, IMemoryCache memoryCache)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _emailService = emailService;
            _memoryCache = memoryCache;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(email);

            // BR-AUTH-001: Email đăng nhập phải tồn tại.
            if (user == null)
                throw new Exception("Email hoặc mật khẩu không chính xác.");

            // BR-AUTH-002: Mật khẩu phải chính xác.
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new Exception("Email hoặc mật khẩu không chính xác.");

            // BR-AUTH-003: Chỉ tài khoản đang hoạt động mới được đăng nhập.
            if (!string.Equals(user.Status, "active", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Tài khoản hiện không hoạt động.");

            var token = _jwtService.GenerateToken(user);

            return new LoginResponseDto
            {
                UserID = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(12)
            };
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            var fullName = dto.FullName.Trim();
            var email = dto.Email.Trim().ToLowerInvariant();
            var phone = dto.Phone.Trim();
            var gender = string.IsNullOrWhiteSpace(dto.Gender) ? null : dto.Gender.Trim().ToLowerInvariant();

            // BR-AUTH-004: Email không được trùng.
            var existingEmail = await _userRepository.GetByEmailAsync(email);

            if (existingEmail != null)
                throw new Exception("Email đã tồn tại.");

            // BR-AUTH-005: Số điện thoại không được trùng.
            var existingPhone = await _userRepository.GetByPhoneAsync(phone);

            if (existingPhone != null)
                throw new Exception("Số điện thoại đã tồn tại.");

            // BR-AUTH-006: Người đăng ký phải từ đủ 16 tuổi.
            if (dto.DateOfBirth.HasValue)
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                var birthDate = dto.DateOfBirth.Value;

                if (birthDate > today)
                    throw new Exception("Ngày sinh không hợp lệ.");

                var age = today.Year - birthDate.Year;

                if (birthDate > today.AddYears(-age))
                    age--;

                if (age < 16)
                    throw new Exception("Người dùng phải từ đủ 16 tuổi.");
            }

            // BR-AUTH-007: Giới tính phải thuộc danh sách hệ thống hỗ trợ.
            if (gender != null && gender != "male" && gender != "female" && gender != "other")
                throw new Exception("Giới tính không hợp lệ.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                Email = email,
                Phone = phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                DateOfBirth = dto.DateOfBirth,
                Gender = gender,
                AvatarUrl = null,
                Role = "patient",
                Status = "active",
                EmailVerifiedAt = null,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return "Đăng ký thành công.";
        }

        public async Task<string> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(email);

            // BR-AUTH-008: Chỉ email đã đăng ký mới được yêu cầu đặt lại mật khẩu.
            if (user == null)
                throw new Exception("Email chưa được đăng ký trong hệ thống.");

            // BR-AUTH-009: Tài khoản phải đang hoạt động.
            if (!string.Equals(user.Status, "active", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Tài khoản hiện không hoạt động.");

            var cooldownKey = GetCooldownKey(email);

            // BR-AUTH-010: Không gửi OTP liên tục trong vòng 60 giây.
            if (_memoryCache.TryGetValue(cooldownKey, out _))
                throw new Exception("Vui lòng chờ 60 giây trước khi yêu cầu mã OTP mới.");

            var otp = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var state = new PasswordResetOtpState
            {
                OtpHash = BCrypt.Net.BCrypt.HashPassword(otp),
                FailedAttempts = 0
            };

            _memoryCache.Set(GetOtpKey(email), state, TimeSpan.FromMinutes(OtpExpiryMinutes));
            _memoryCache.Set(cooldownKey, true, TimeSpan.FromSeconds(OtpResendSeconds));

            try
            {
                await _emailService.SendPasswordResetOtpAsync(user.Email, user.FullName, otp);
            }
            catch
            {
                _memoryCache.Remove(GetOtpKey(email));
                _memoryCache.Remove(cooldownKey);
                throw;
            }

            return "Mã OTP đã được gửi đến email của bạn. Mã có hiệu lực trong 5 phút.";
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(email);

            // BR-AUTH-011: Tài khoản phải tồn tại và đang hoạt động.
            if (user == null)
                throw new Exception("Không tìm thấy tài khoản.");

            if (!string.Equals(user.Status, "active", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Tài khoản hiện không hoạt động.");

            var otpKey = GetOtpKey(email);

            // BR-AUTH-012: OTP phải tồn tại và còn hiệu lực.
            if (!_memoryCache.TryGetValue<PasswordResetOtpState>(otpKey, out var state) || state == null)
                throw new Exception("Mã OTP không tồn tại hoặc đã hết hạn. Vui lòng gửi mã mới.");

            // BR-AUTH-013: OTP chỉ được nhập sai tối đa 5 lần.
            if (state.FailedAttempts >= OtpMaximumAttempts)
            {
                _memoryCache.Remove(otpKey);
                throw new Exception("Bạn đã nhập sai OTP quá số lần cho phép. Vui lòng gửi mã mới.");
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Otp.Trim(), state.OtpHash))
            {
                state.FailedAttempts++;

                if (state.FailedAttempts >= OtpMaximumAttempts)
                {
                    _memoryCache.Remove(otpKey);
                    throw new Exception("Bạn đã nhập sai OTP quá số lần cho phép. Vui lòng gửi mã mới.");
                }

                throw new Exception($"Mã OTP không chính xác. Bạn còn {OtpMaximumAttempts - state.FailedAttempts} lần thử.");
            }

            // BR-AUTH-014: Mật khẩu mới không được trùng mật khẩu hiện tại.
            if (BCrypt.Net.BCrypt.Verify(dto.NewPassword, user.PasswordHash))
                throw new Exception("Mật khẩu mới không được trùng với mật khẩu hiện tại.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.Now;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            // BR-AUTH-015: OTP chỉ được sử dụng một lần.
            _memoryCache.Remove(otpKey);
            _memoryCache.Remove(GetCooldownKey(email));

            return "Đặt lại mật khẩu thành công.";
        }

        private static string GetOtpKey(string email)
        {
            return $"password-reset-otp:{email}";
        }

        private static string GetCooldownKey(string email)
        {
            return $"password-reset-cooldown:{email}";
        }

        private sealed class PasswordResetOtpState
        {
            public string OtpHash { get; set; } = string.Empty;

            public int FailedAttempts { get; set; }
        }
    }
}