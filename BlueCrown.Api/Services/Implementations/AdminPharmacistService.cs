using BlueCrown.Api.DTOs.Users;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Services.Implementations
{
    public class AdminPharmacistService : IAdminPharmacistService
    {
        private static readonly string[] AllowedStatuses =
            ["active", "suspended", "pending"];

        private readonly IUserRepository _userRepository;
        private readonly BlueCrownContext _context;

        public AdminPharmacistService(
            IUserRepository userRepository,
            BlueCrownContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync(
            string? search = null,
            string? status = null)
        {
            var users = await _userRepository.GetAllAsync();

            users = users.Where(u =>
                string.Equals(
                    u.Role,
                    "pharmacist",
                    StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim();

                users = users.Where(u =>
                    u.FullName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (u.Phone != null &&
                     u.Phone.Contains(keyword, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.Trim().ToLowerInvariant();
                ValidateStatus(status);

                users = users.Where(u =>
                    string.Equals(
                        u.Status,
                        status,
                        StringComparison.OrdinalIgnoreCase));
            }

            return users
                .OrderByDescending(u => u.CreatedAt)
                .Select(MapUser);
        }

        public async Task<UserDetailDto?> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return null;

            EnsurePharmacist(user);

            return MapDetail(user);
        }

        public async Task<UserDetailDto> CreateAsync(
            AdminPharmacistCreateDto dto)
        {
            var fullName = dto.FullName.Trim();
            var email = dto.Email.Trim().ToLowerInvariant();
            var phone = dto.Phone.Trim();
            var gender = string.IsNullOrWhiteSpace(dto.Gender)
                ? null
                : dto.Gender.Trim().ToLowerInvariant();
            var status = dto.Status.Trim().ToLowerInvariant();

            ValidateStatus(status);
            ValidateDateOfBirth(dto.DateOfBirth);

            if (await _userRepository.GetByEmailAsync(email) != null)
                throw new InvalidOperationException("Email đã tồn tại.");

            if (await _userRepository.GetByPhoneAsync(phone) != null)
                throw new InvalidOperationException("Số điện thoại đã tồn tại.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                Email = email,
                Phone = phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                DateOfBirth = dto.DateOfBirth,
                Gender = gender,
                Role = "pharmacist",
                Status = status,
                AvatarUrl = null,
                EmailVerifiedAt = null,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return MapDetail(user);
        }

        public async Task<UserDetailDto> UpdateAsync(
            Guid id,
            AdminPharmacistUpdateDto dto,
            Guid currentAdminId)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException("Không tìm thấy dược sĩ.");

            EnsurePharmacist(user);

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.FullName = dto.FullName.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var email = dto.Email.Trim().ToLowerInvariant();
                var existing = await _userRepository.GetByEmailAsync(email);

                if (existing != null && existing.Id != id)
                    throw new InvalidOperationException(
                        "Email đã được sử dụng bởi tài khoản khác.");

                user.Email = email;
            }

            if (!string.IsNullOrWhiteSpace(dto.Phone))
            {
                var phone = dto.Phone.Trim();
                var existing = await _userRepository.GetByPhoneAsync(phone);

                if (existing != null && existing.Id != id)
                    throw new InvalidOperationException(
                        "Số điện thoại đã được sử dụng bởi tài khoản khác.");

                user.Phone = phone;
            }

            if (dto.DateOfBirth.HasValue)
            {
                ValidateDateOfBirth(dto.DateOfBirth);
                user.DateOfBirth = dto.DateOfBirth;
            }

            if (dto.Gender != null)
            {
                user.Gender = string.IsNullOrWhiteSpace(dto.Gender)
                    ? null
                    : dto.Gender.Trim().ToLowerInvariant();
            }

            if (dto.AvatarUrl != null)
            {
                user.AvatarUrl = string.IsNullOrWhiteSpace(dto.AvatarUrl)
                    ? null
                    : dto.AvatarUrl.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                var status = dto.Status.Trim().ToLowerInvariant();
                ValidateStatus(status);

                if (id == currentAdminId && status != "active")
                    throw new InvalidOperationException(
                        "Bạn không thể tự khóa tài khoản Admin đang đăng nhập.");

                user.Status = status;
            }

            user.UpdatedAt = DateTime.Now;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return MapDetail(user);
        }

        public async Task<UserDetailDto> UpdateStatusAsync(
            Guid id,
            UpdateUserStatusDto dto,
            Guid currentAdminId)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException("Không tìm thấy dược sĩ.");

            EnsurePharmacist(user);

            var status = dto.Status.Trim().ToLowerInvariant();
            ValidateStatus(status);

            if (id == currentAdminId && status != "active")
                throw new InvalidOperationException(
                    "Bạn không thể tự khóa tài khoản Admin đang đăng nhập.");

            user.Status = status;
            user.UpdatedAt = DateTime.Now;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return MapDetail(user);
        }

        public async Task<string> DeleteAsync(
            Guid id,
            Guid currentAdminId)
        {
            if (id == currentAdminId)
                throw new InvalidOperationException(
                    "Bạn không thể xóa tài khoản Admin đang đăng nhập.");

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException("Không tìm thấy dược sĩ.");

            EnsurePharmacist(user);

            var hasBusinessData =
                await _context.ChatMessages.AnyAsync(x => x.SenderId == id) ||
                await _context.InventoryReceipts.AnyAsync(
                    x => x.ApprovedBy == id || x.CreatedBy == id) ||
                await _context.PrescriptionDispenseItems.AnyAsync(
                    x => x.DispensedBy == id) ||
                await _context.Notifications.AnyAsync(x => x.UserId == id);

            if (hasBusinessData)
                throw new InvalidOperationException(
                    "Dược sĩ đã có dữ liệu nghiệp vụ nên không thể xóa. Vui lòng khóa tài khoản thay vì xóa.");

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Users.Remove(user);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return "Xóa tài khoản dược sĩ thành công.";
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static void EnsurePharmacist(User user)
        {
            if (!string.Equals(
                    user.Role,
                    "pharmacist",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Chức năng này chỉ dành cho tài khoản dược sĩ.");
            }
        }

        private static UserDto MapUser(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                Status = user.Status
            };
        }

        private static UserDetailDto MapDetail(User user)
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

        private static void ValidateStatus(string status)
        {
            if (!AllowedStatuses.Contains(status))
                throw new InvalidOperationException(
                    "Trạng thái tài khoản không hợp lệ.");
        }

        private static void ValidateDateOfBirth(DateOnly? dateOfBirth)
        {
            if (dateOfBirth.HasValue &&
                dateOfBirth.Value > DateOnly.FromDateTime(DateTime.Today))
            {
                throw new InvalidOperationException(
                    "Ngày sinh không hợp lệ.");
            }
        }
    }
}