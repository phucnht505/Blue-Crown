using BlueCrown.Api.DTOs.Users;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class UserService : IUserService
    {
        private static readonly string[] AllowedRoles = ["patient", "doctor", "pharmacist", "admin"];
        private static readonly string[] AllowedStatuses = ["active", "suspended", "pending"];
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync(string? search = null, string? role = null, string? status = null)
        {
            var users = await _userRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim();

                users = users.Where(u =>
                    u.FullName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (u.Phone != null && u.Phone.Contains(keyword, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                role = role.Trim().ToLowerInvariant();
                ValidateRole(role);
                users = users.Where(u => string.Equals(u.Role, role, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.Trim().ToLowerInvariant();
                ValidateStatus(status);
                users = users.Where(u => string.Equals(u.Status, status, StringComparison.OrdinalIgnoreCase));
            }

            return users.OrderByDescending(u => u.CreatedAt).Select(MapUser);
        }

        public async Task<UserDetailDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : MapDetail(user);
        }

        public async Task<UserDetailDto> CreateUserByAdminAsync(AdminCreateUserDto dto)
        {
            var fullName = dto.FullName.Trim();
            var email = dto.Email.Trim().ToLowerInvariant();
            var phone = dto.Phone.Trim();
            var gender = string.IsNullOrWhiteSpace(dto.Gender) ? null : dto.Gender.Trim().ToLowerInvariant();
            var role = dto.Role.Trim().ToLowerInvariant();
            var status = dto.Status.Trim().ToLowerInvariant();

            ValidateRole(role);
            ValidateStatus(status);

            // BR-USER-001: Email tài khoản phải duy nhất.
            if (await _userRepository.GetByEmailAsync(email) != null)
                throw new InvalidOperationException("Email đã tồn tại.");

            // BR-USER-002: Số điện thoại tài khoản phải duy nhất.
            if (await _userRepository.GetByPhoneAsync(phone) != null)
                throw new InvalidOperationException("Số điện thoại đã tồn tại.");

            ValidateDateOfBirth(dto.DateOfBirth);

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                Email = email,
                Phone = phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                DateOfBirth = dto.DateOfBirth,
                Gender = gender,
                Role = role,
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

        public async Task<UserDetailDto> UpdateUserByAdminAsync(Guid id, AdminUpdateUserDto dto, Guid currentAdminId)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException("Không tìm thấy người dùng.");

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.FullName = dto.FullName.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var email = dto.Email.Trim().ToLowerInvariant();
                var existing = await _userRepository.GetByEmailAsync(email);

                // BR-USER-003: Không được cập nhật sang email của tài khoản khác.
                if (existing != null && existing.Id != id)
                    throw new InvalidOperationException("Email đã được sử dụng bởi tài khoản khác.");

                user.Email = email;
            }

            if (!string.IsNullOrWhiteSpace(dto.Phone))
            {
                var phone = dto.Phone.Trim();
                var existing = await _userRepository.GetByPhoneAsync(phone);

                if (existing != null && existing.Id != id)
                    throw new InvalidOperationException("Số điện thoại đã được sử dụng bởi tài khoản khác.");

                user.Phone = phone;
            }

            if (dto.DateOfBirth.HasValue)
            {
                ValidateDateOfBirth(dto.DateOfBirth);
                user.DateOfBirth = dto.DateOfBirth;
            }

            if (dto.Gender != null)
                user.Gender = string.IsNullOrWhiteSpace(dto.Gender) ? null : dto.Gender.Trim().ToLowerInvariant();

            if (dto.AvatarUrl != null)
                user.AvatarUrl = string.IsNullOrWhiteSpace(dto.AvatarUrl) ? null : dto.AvatarUrl.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                var role = dto.Role.Trim().ToLowerInvariant();
                ValidateRole(role);

                // BR-USER-004: Admin đang đăng nhập không được tự hạ quyền của chính mình.
                if (id == currentAdminId && role != "admin")
                    throw new InvalidOperationException("Bạn không thể thay đổi vai trò của chính tài khoản Admin đang đăng nhập.");

                user.Role = role;
            }

            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                var status = dto.Status.Trim().ToLowerInvariant();
                ValidateStatus(status);

                // BR-USER-005: Admin không được tự khóa tài khoản đang sử dụng.
                if (id == currentAdminId && status != "active")
                    throw new InvalidOperationException("Bạn không thể tự khóa tài khoản Admin đang đăng nhập.");

                user.Status = status;
            }

            user.UpdatedAt = DateTime.Now;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return MapDetail(user);
        }

        public async Task<UserDetailDto> UpdateUserStatusAsync(Guid id, UpdateUserStatusDto dto, Guid currentAdminId)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException("Không tìm thấy người dùng.");

            var status = dto.Status.Trim().ToLowerInvariant();
            ValidateStatus(status);

            if (id == currentAdminId && status != "active")
                throw new InvalidOperationException("Bạn không thể tự khóa tài khoản Admin đang đăng nhập.");

            user.Status = status;
            user.UpdatedAt = DateTime.Now;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return MapDetail(user);
        }

        public async Task<string> DeleteUserByAdminAsync(Guid id, Guid currentAdminId)
        {
            if (id == currentAdminId)
                throw new InvalidOperationException("Bạn không thể xóa tài khoản Admin đang đăng nhập.");

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException("Không tìm thấy người dùng.");

            // BR-USER-006: Không xóa vật lý tài khoản để bảo toàn dữ liệu y tế và nghiệp vụ liên quan.
            user.Status = "suspended";
            user.UpdatedAt = DateTime.Now;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return "Tài khoản đã được vô hiệu hóa để bảo toàn dữ liệu liên quan.";
        }

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();
            var phone = dto.Phone.Trim();

            if (await _userRepository.GetByEmailAsync(email) != null || await _userRepository.GetByPhoneAsync(phone) != null)
                return false;

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName.Trim(),
                Email = email,
                Phone = phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                DateOfBirth = dto.DateOfBirth,
                Gender = string.IsNullOrWhiteSpace(dto.Gender) ? null : dto.Gender.Trim().ToLowerInvariant(),
                Role = "patient",
                Status = "active",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateUserAsync(Guid id, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return false;

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.FullName = dto.FullName.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Phone))
            {
                var existing = await _userRepository.GetByPhoneAsync(dto.Phone.Trim());

                if (existing != null && existing.Id != id)
                    return false;

                user.Phone = dto.Phone.Trim();
            }

            if (dto.DateOfBirth.HasValue)
                user.DateOfBirth = dto.DateOfBirth;

            if (dto.Gender != null)
                user.Gender = string.IsNullOrWhiteSpace(dto.Gender) ? null : dto.Gender.Trim().ToLowerInvariant();

            if (dto.AvatarUrl != null)
                user.AvatarUrl = string.IsNullOrWhiteSpace(dto.AvatarUrl) ? null : dto.AvatarUrl.Trim();

            user.UpdatedAt = DateTime.Now;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return false;

            user.Status = "suspended";
            user.UpdatedAt = DateTime.Now;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
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

        private static void ValidateRole(string role)
        {
            if (!AllowedRoles.Contains(role))
                throw new InvalidOperationException("Vai trò không hợp lệ.");
        }

        private static void ValidateStatus(string status)
        {
            if (!AllowedStatuses.Contains(status))
                throw new InvalidOperationException("Trạng thái tài khoản không hợp lệ.");
        }

        private static void ValidateDateOfBirth(DateOnly? dateOfBirth)
        {
            if (dateOfBirth.HasValue && dateOfBirth.Value > DateOnly.FromDateTime(DateTime.Today))
                throw new InvalidOperationException("Ngày sinh không hợp lệ.");
        }
    }
}