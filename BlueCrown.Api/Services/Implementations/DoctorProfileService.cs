using BlueCrown.Api.DTOs.DoctorProfiles;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class DoctorProfileService : IDoctorProfileService
    {
        private readonly IDoctorProfileRepository _repository;
        private readonly IUserRepository _userRepository;

        public DoctorProfileService(IDoctorProfileRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public async Task<List<DoctorProfileDto>> GetAllAsync(string? search = null, string? specialty = null, string? status = null)
        {
            var doctors = await _repository.GetAllAsync(search, specialty, status);
            return doctors.Select(MapToDto).ToList();
        }

        public async Task<DoctorProfileDto?> GetByIdAsync(Guid id)
        {
            var doctor = await _repository.GetByIdAsync(id);
            return doctor == null ? null : MapToDto(doctor);
        }

        public async Task<DoctorProfileDto?> GetByUserIdAsync(Guid userId)
        {
            var doctor = await _repository.GetByUserIdAsync(userId);
            return doctor == null ? null : MapToDto(doctor);
        }

        public async Task<DoctorProfileDto> CreateAsync(Guid userId, CreateDoctorProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new InvalidOperationException("Không tìm thấy tài khoản.");

            if (!string.Equals(user.Role, "doctor", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Chỉ tài khoản bác sĩ mới được tạo hồ sơ bác sĩ.");

            if (await _repository.GetByUserIdAsync(userId) != null)
                throw new InvalidOperationException("Tài khoản này đã có hồ sơ bác sĩ.");

            var specialty = NormalizeRequired(dto.Specialty, "Chuyên khoa");
            var licenseNumber = NormalizeRequired(dto.LicenseNumber, "Số giấy phép");

            if (await _repository.GetByLicenseNumberAsync(licenseNumber) != null)
                throw new InvalidOperationException("Số giấy phép hành nghề đã tồn tại.");

            await ValidateClinicAsync(dto.ClinicId);
            ValidateProfessionalInfo(dto.YearsExperience, dto.ConsultationFee);

            var profile = new DoctorProfile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Specialty = specialty,
                LicenseNumber = licenseNumber,
                LicenseVerified = false,
                Bio = NormalizeOptional(dto.Bio),
                YearsExperience = dto.YearsExperience,
                ClinicId = dto.ClinicId,
                ConsultationFee = dto.ConsultationFee,
                RatingAvg = 0,
                RatingCount = 0
            };

            await _repository.AddAsync(profile);
            await _repository.SaveChangesAsync();

            var created = await _repository.GetByIdAsync(profile.Id);

            if (created == null)
                throw new InvalidOperationException("Không thể tải lại hồ sơ bác sĩ vừa tạo.");

            return MapToDto(created);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateDoctorProfileDto dto)
        {
            var doctor = await _repository.GetByIdAsync(id);

            if (doctor == null)
                return false;

            await ValidateClinicAsync(dto.ClinicId);
            ValidateProfessionalInfo(dto.YearsExperience, dto.ConsultationFee);

            doctor.Specialty = NormalizeRequired(dto.Specialty, "Chuyên khoa");
            doctor.Bio = NormalizeOptional(dto.Bio);
            doctor.YearsExperience = dto.YearsExperience;
            doctor.ClinicId = dto.ClinicId;
            doctor.ConsultationFee = dto.ConsultationFee;

            await _repository.UpdateAsync(doctor);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> VerifyLicenseAsync(Guid id, bool verified)
        {
            var doctor = await _repository.GetByIdAsync(id);

            if (doctor == null)
                return false;

            doctor.LicenseVerified = verified;

            await _repository.UpdateAsync(doctor);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<AdminDoctorMetaDto> GetAdminMetaAsync()
        {
            var clinics = await _repository.GetClinicsAsync();
            var specialties = await _repository.GetSpecialtiesAsync();

            return new AdminDoctorMetaDto
            {
                Specialties = specialties,
                Clinics = clinics.Select(c => new DoctorClinicOptionDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Address = c.Address,
                    Phone = c.Phone
                }).ToList()
            };
        }

        public async Task<DoctorProfileDto> AdminCreateAsync(AdminCreateDoctorDto dto)
        {
            var fullName = NormalizeRequired(dto.FullName, "Họ tên");
            var email = NormalizeRequired(dto.Email, "Email").ToLowerInvariant();
            var phone = NormalizeRequired(dto.Phone, "Số điện thoại");
            var specialty = NormalizeRequired(dto.Specialty, "Chuyên khoa");
            var licenseNumber = NormalizeRequired(dto.LicenseNumber, "Số giấy phép");
            var status = dto.Status.Trim().ToLowerInvariant();
            var gender = NormalizeOptional(dto.Gender)?.ToLowerInvariant();

            ValidateStatus(status);
            ValidateGender(gender);
            ValidateDateOfBirth(dto.DateOfBirth);
            ValidateProfessionalInfo(dto.YearsExperience, dto.ConsultationFee);
            await ValidateClinicAsync(dto.ClinicId);

            // BR-DOCTOR-001: Email và số điện thoại của bác sĩ phải duy nhất.
            if (await _userRepository.GetByEmailAsync(email) != null)
                throw new InvalidOperationException("Email đã tồn tại.");

            if (await _userRepository.GetByPhoneAsync(phone) != null)
                throw new InvalidOperationException("Số điện thoại đã tồn tại.");

            // BR-DOCTOR-002: Số giấy phép hành nghề phải duy nhất.
            if (await _repository.GetByLicenseNumberAsync(licenseNumber) != null)
                throw new InvalidOperationException("Số giấy phép hành nghề đã tồn tại.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                Email = email,
                Phone = phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                DateOfBirth = dto.DateOfBirth,
                Gender = gender,
                AvatarUrl = NormalizeOptional(dto.AvatarUrl),
                Role = "doctor",
                Status = status,
                EmailVerifiedAt = null,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var doctor = new DoctorProfile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Specialty = specialty,
                LicenseNumber = licenseNumber,
                LicenseVerified = dto.LicenseVerified,
                Bio = NormalizeOptional(dto.Bio),
                YearsExperience = dto.YearsExperience,
                ClinicId = dto.ClinicId,
                ConsultationFee = dto.ConsultationFee,
                RatingAvg = 0,
                RatingCount = 0
            };

            await _userRepository.AddAsync(user);
            await _repository.AddAsync(doctor);

            // Hai entity dùng cùng DbContext scoped nên SaveChanges lưu User + DoctorProfile trong cùng transaction.
            await _repository.SaveChangesAsync();

            var created = await _repository.GetByIdAsync(doctor.Id);

            if (created == null)
                throw new InvalidOperationException("Không thể tải lại bác sĩ vừa tạo.");

            return MapToDto(created);
        }

        public async Task<DoctorProfileDto> AdminUpdateAsync(Guid id, AdminUpdateDoctorDto dto)
        {
            var doctor = await _repository.GetByIdAsync(id);

            if (doctor == null)
                throw new KeyNotFoundException("Không tìm thấy bác sĩ.");

            var fullName = NormalizeRequired(dto.FullName, "Họ tên");
            var email = NormalizeRequired(dto.Email, "Email").ToLowerInvariant();
            var phone = NormalizeRequired(dto.Phone, "Số điện thoại");
            var specialty = NormalizeRequired(dto.Specialty, "Chuyên khoa");
            var licenseNumber = NormalizeRequired(dto.LicenseNumber, "Số giấy phép");
            var status = dto.Status.Trim().ToLowerInvariant();
            var gender = NormalizeOptional(dto.Gender)?.ToLowerInvariant();

            ValidateStatus(status);
            ValidateGender(gender);
            ValidateDateOfBirth(dto.DateOfBirth);
            ValidateProfessionalInfo(dto.YearsExperience, dto.ConsultationFee);
            await ValidateClinicAsync(dto.ClinicId);

            var existingEmail = await _userRepository.GetByEmailAsync(email);

            if (existingEmail != null && existingEmail.Id != doctor.UserId)
                throw new InvalidOperationException("Email đã được sử dụng bởi tài khoản khác.");

            var existingPhone = await _userRepository.GetByPhoneAsync(phone);

            if (existingPhone != null && existingPhone.Id != doctor.UserId)
                throw new InvalidOperationException("Số điện thoại đã được sử dụng bởi tài khoản khác.");

            var existingLicense = await _repository.GetByLicenseNumberAsync(licenseNumber);

            if (existingLicense != null && existingLicense.Id != doctor.Id)
                throw new InvalidOperationException("Số giấy phép hành nghề đã được sử dụng bởi bác sĩ khác.");

            doctor.User.FullName = fullName;
            doctor.User.Email = email;
            doctor.User.Phone = phone;
            doctor.User.DateOfBirth = dto.DateOfBirth;
            doctor.User.Gender = gender;
            doctor.User.AvatarUrl = NormalizeOptional(dto.AvatarUrl);
            doctor.User.Status = status;
            doctor.User.Role = "doctor";
            doctor.User.UpdatedAt = DateTime.Now;

            doctor.Specialty = specialty;
            doctor.LicenseNumber = licenseNumber;
            doctor.LicenseVerified = dto.LicenseVerified;
            doctor.Bio = NormalizeOptional(dto.Bio);
            doctor.YearsExperience = dto.YearsExperience;
            doctor.ClinicId = dto.ClinicId;
            doctor.ConsultationFee = dto.ConsultationFee;

            await _repository.UpdateAsync(doctor);
            await _repository.SaveChangesAsync();

            var updated = await _repository.GetByIdAsync(id);

            if (updated == null)
                throw new InvalidOperationException("Không thể tải lại thông tin bác sĩ.");

            return MapToDto(updated);
        }

        public async Task<DoctorProfileDto> AdminUpdateStatusAsync(Guid id, UpdateDoctorStatusDto dto)
        {
            var doctor = await _repository.GetByIdAsync(id);

            if (doctor == null)
                throw new KeyNotFoundException("Không tìm thấy bác sĩ.");

            var status = dto.Status.Trim().ToLowerInvariant();
            ValidateStatus(status);

            // BR-DOCTOR-003: Khóa bác sĩ bằng trạng thái tài khoản, không xóa dữ liệu chuyên môn.
            doctor.User.Status = status;
            doctor.User.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(doctor);
            await _repository.SaveChangesAsync();

            return MapToDto(doctor);
        }

        public async Task<string> AdminDeactivateAsync(Guid id)
        {
            var doctor = await _repository.GetByIdAsync(id);

            if (doctor == null)
                throw new KeyNotFoundException("Không tìm thấy bác sĩ.");

            doctor.User.Status = "suspended";
            doctor.User.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(doctor);
            await _repository.SaveChangesAsync();

            return "Bác sĩ đã được vô hiệu hóa. Hồ sơ, lịch khám và dữ liệu y tế liên quan vẫn được giữ lại.";
        }

        private async Task ValidateClinicAsync(Guid? clinicId)
        {
            if (clinicId.HasValue && !await _repository.ClinicExistsAsync(clinicId.Value))
                throw new InvalidOperationException("Phòng khám không tồn tại.");
        }

        private static void ValidateProfessionalInfo(int? yearsExperience, decimal? consultationFee)
        {
            if (yearsExperience.HasValue && (yearsExperience < 0 || yearsExperience > 80))
                throw new InvalidOperationException("Số năm kinh nghiệm phải từ 0 đến 80.");

            if (consultationFee.HasValue && consultationFee < 0)
                throw new InvalidOperationException("Phí khám không được nhỏ hơn 0.");
        }

        private static void ValidateStatus(string status)
        {
            if (status != "active" && status != "suspended" && status != "pending")
                throw new InvalidOperationException("Trạng thái tài khoản không hợp lệ.");
        }

        private static void ValidateGender(string? gender)
        {
            if (gender != null && gender != "male" && gender != "female" && gender != "other")
                throw new InvalidOperationException("Giới tính không hợp lệ.");
        }

        private static void ValidateDateOfBirth(DateOnly? dateOfBirth)
        {
            if (!dateOfBirth.HasValue)
                return;

            var today = DateOnly.FromDateTime(DateTime.Today);

            if (dateOfBirth.Value > today)
                throw new InvalidOperationException("Ngày sinh không hợp lệ.");

            var age = today.Year - dateOfBirth.Value.Year;

            if (dateOfBirth.Value > today.AddYears(-age))
                age--;

            if (age < 18)
                throw new InvalidOperationException("Bác sĩ phải từ đủ 18 tuổi.");
        }

        private static string NormalizeRequired(string value, string fieldName)
        {
            var result = value?.Trim();

            if (string.IsNullOrWhiteSpace(result))
                throw new InvalidOperationException($"{fieldName} không được để trống.");

            return result;
        }

        private static string? NormalizeOptional(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static DoctorProfileDto MapToDto(DoctorProfile doctor)
        {
            return new DoctorProfileDto
            {
                Id = doctor.Id,
                UserId = doctor.UserId,
                FullName = doctor.User?.FullName,
                Email = doctor.User?.Email,
                Phone = doctor.User?.Phone,
                DateOfBirth = doctor.User?.DateOfBirth,
                Gender = doctor.User?.Gender,
                AvatarUrl = doctor.User?.AvatarUrl,
                UserStatus = doctor.User?.Status,
                Specialty = doctor.Specialty,
                LicenseNumber = doctor.LicenseNumber,
                LicenseVerified = doctor.LicenseVerified,
                Bio = doctor.Bio,
                YearsExperience = doctor.YearsExperience,
                ClinicId = doctor.ClinicId,
                ClinicName = doctor.Clinic?.Name,
                ClinicAddress = doctor.Clinic?.Address,
                ClinicPhone = doctor.Clinic?.Phone,
                ConsultationFee = doctor.ConsultationFee,
                RatingAvg = doctor.RatingAvg,
                RatingCount = doctor.RatingCount
            };
        }
    }
}