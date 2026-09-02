using BlueCrown.Api.DTOs.Users;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;

        public AccountService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDetailDto?> GetMyProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user == null ? null : MapToDto(user);
        }

        public async Task<UserDetailDto> UpdateMyProfileAsync(Guid userId, UpdateAccountProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("Không tìm thấy tài khoản.");

            var fullName = dto.FullName.Trim();
            var phone = dto.Phone.Trim();
            var gender = string.IsNullOrWhiteSpace(dto.Gender) ? null : dto.Gender.Trim().ToLowerInvariant();

            ValidateDateOfBirth(dto.DateOfBirth);
            ValidateGender(gender);

            var existingPhone = await _userRepository.GetByPhoneAsync(phone);

            // BR-ACCOUNT-001: Số điện thoại phải duy nhất giữa các tài khoản.
            if (existingPhone != null && existingPhone.Id != userId)
                throw new InvalidOperationException("Số điện thoại đã được sử dụng bởi tài khoản khác.");

            user.FullName = fullName;
            user.Phone = phone;
            user.DateOfBirth = dto.DateOfBirth;
            user.Gender = gender;
            user.AvatarUrl = string.IsNullOrWhiteSpace(dto.AvatarUrl) ? null : dto.AvatarUrl.Trim();
            user.UpdatedAt = DateTime.Now;

            // BR-ACCOUNT-002: Người dùng không được tự thay đổi Email, Role hoặc Status tại hồ sơ cá nhân.
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return MapToDto(user);
        }

        private static void ValidateDateOfBirth(DateOnly? dateOfBirth)
        {
            if (dateOfBirth.HasValue && dateOfBirth.Value > DateOnly.FromDateTime(DateTime.Today))
                throw new InvalidOperationException("Ngày sinh không hợp lệ.");
        }

        private static void ValidateGender(string? gender)
        {
            if (gender != null && gender != "male" && gender != "female" && gender != "other")
                throw new InvalidOperationException("Giới tính không hợp lệ.");
        }

        private static UserDetailDto MapToDto(User user)
        {
            return new UserDetailDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                AvatarUrl = user.AvatarUrl,
                Role = user.Role,
                Status = user.Status
            };
        }
    }
}