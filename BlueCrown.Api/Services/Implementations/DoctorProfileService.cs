using BlueCrown.Api.DTOs.DoctorProfiles;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Services.Implementations
{
    public class DoctorProfileService : IDoctorProfileService
    {
        private readonly IDoctorProfileRepository _repository;
        private readonly BlueCrownContext _context;

        public DoctorProfileService(
            IDoctorProfileRepository repository,
            BlueCrownContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<List<DoctorProfileDto>> GetAllAsync()
        {
            var doctors = await _repository.GetAllAsync();

            return doctors
                .Select(MapToDto)
                .ToList();
        }

        public async Task<DoctorProfileDto?> GetByIdAsync(Guid id)
        {
            var doctor = await _repository.GetByIdAsync(id);

            if (doctor == null)
                return null;

            return MapToDto(doctor);
        }

        public async Task<DoctorProfileDto?> GetByUserIdAsync(Guid userId)
        {
            var doctor = await _repository.GetByUserIdAsync(userId);

            if (doctor == null)
                return null;

            return MapToDto(doctor);
        }

        public async Task<DoctorProfileDto> CreateAsync(
            Guid userId,
            CreateDoctorProfileDto dto)
        {
            // Kiểm tra User có tồn tại không
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Tài khoản phải là Doctor
            if (!string.Equals(user.Role, "Doctor", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Only users with Doctor role can create a doctor profile.");
            }

            // Một User chỉ có một DoctorProfile
            var existingProfile = await _repository.GetByUserIdAsync(userId);

            if (existingProfile != null)
            {
                throw new Exception(
                    "Doctor profile already exists for this user.");
            }

            // Kiểm tra Clinic nếu có
            if (dto.ClinicId.HasValue)
            {
                var clinicExists = await _context.Clinics
                    .AnyAsync(c => c.Id == dto.ClinicId.Value);

                if (!clinicExists)
                {
                    throw new Exception("Clinic not found.");
                }
            }

            var doctorProfile = new DoctorProfile
            {
                Id = Guid.NewGuid(),

                UserId = userId,

                Specialty = dto.Specialty,

                LicenseNumber = dto.LicenseNumber,

                LicenseVerified = false,

                Bio = dto.Bio,

                YearsExperience = dto.YearsExperience,

                ClinicId = dto.ClinicId,

                ConsultationFee = dto.ConsultationFee,

                RatingAvg = 0,

                RatingCount = 0
            };

            await _repository.AddAsync(doctorProfile);

            await _repository.SaveChangesAsync();

            var createdDoctor =
                await _repository.GetByIdAsync(doctorProfile.Id);

            if (createdDoctor == null)
            {
                throw new Exception(
                    "Failed to retrieve created doctor profile.");
            }

            return MapToDto(createdDoctor);
        }

        public async Task<bool> UpdateAsync(
            Guid id,
            UpdateDoctorProfileDto dto)
        {
            var doctorProfile = await _repository.GetByIdAsync(id);

            if (doctorProfile == null)
                return false;

            // Kiểm tra Clinic nếu có
            if (dto.ClinicId.HasValue)
            {
                var clinicExists = await _context.Clinics
                    .AnyAsync(c => c.Id == dto.ClinicId.Value);

                if (!clinicExists)
                {
                    throw new Exception("Clinic not found.");
                }
            }

            doctorProfile.Specialty = dto.Specialty;

            doctorProfile.Bio = dto.Bio;

            doctorProfile.YearsExperience = dto.YearsExperience;

            doctorProfile.ClinicId = dto.ClinicId;

            doctorProfile.ConsultationFee = dto.ConsultationFee;

            await _repository.UpdateAsync(doctorProfile);

            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> VerifyLicenseAsync(
            Guid id,
            bool verified)
        {
            var doctorProfile = await _repository.GetByIdAsync(id);

            if (doctorProfile == null)
                return false;

            doctorProfile.LicenseVerified = verified;

            await _repository.UpdateAsync(doctorProfile);

            await _repository.SaveChangesAsync();

            return true;
        }

        private static DoctorProfileDto MapToDto(
            DoctorProfile doctorProfile)
        {
            return new DoctorProfileDto
            {
                Id = doctorProfile.Id,

                UserId = doctorProfile.UserId,

                Specialty = doctorProfile.Specialty,

                LicenseVerified = doctorProfile.LicenseVerified,

                Bio = doctorProfile.Bio,

                YearsExperience = doctorProfile.YearsExperience,

                ClinicId = doctorProfile.ClinicId,

                ClinicName = doctorProfile.Clinic?.Name,

                ClinicAddress = doctorProfile.Clinic?.Address,

                ClinicPhone = doctorProfile.Clinic?.Phone,

                ConsultationFee = doctorProfile.ConsultationFee,

                RatingAvg = doctorProfile.RatingAvg,

                RatingCount = doctorProfile.RatingCount
            };
        }
    }
}