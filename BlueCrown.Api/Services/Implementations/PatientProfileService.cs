using BlueCrown.Api.DTOs.PatientProfiles;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class PatientProfileService : IPatientProfileService
    {
        private readonly IPatientProfileRepository _repository;

        public PatientProfileService(IPatientProfileRepository repository)
        {
            _repository = repository;
        }

        public async Task<PatientProfileDto?> GetMyProfileAsync(Guid userId)
        {
            var profile = await _repository.GetByUserIdAsync(userId);

            if (profile == null)
                return null;

            return new PatientProfileDto
            {
                Id = profile.Id,
                UserId = profile.UserId,
                BloodType = profile.BloodType,
                HeightCm = profile.HeightCm,
                WeightKg = profile.WeightKg,
                Allergies = profile.Allergies,
                ChronicConditions = profile.ChronicConditions,
                EmergencyContactName = profile.EmergencyContactName,
                EmergencyContactPhone = profile.EmergencyContactPhone
            };
        }

        public async Task CreateProfileAsync(Guid userId, CreatePatientProfileDto dto)
        {
            var existing = await _repository.GetByUserIdAsync(userId);

            if (existing != null)
                throw new Exception("Người dùng đã có hồ sơ sức khỏe");

            var profile = new PatientProfile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BloodType = dto.BloodType,
                HeightCm = dto.HeightCm,
                WeightKg = dto.WeightKg,
                Allergies = dto.Allergies,
                ChronicConditions = dto.ChronicConditions,
                EmergencyContactName = dto.EmergencyContactName,
                EmergencyContactPhone = dto.EmergencyContactPhone
            };

            await _repository.AddAsync(profile);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateProfileAsync(Guid userId, UpdatePatientProfileDto dto)
        {
            var profile = await _repository.GetByUserIdAsync(userId);

            if (profile == null)
                throw new Exception("Không tìm thấy hồ sơ!");

            profile.BloodType = dto.BloodType;
            profile.HeightCm = dto.HeightCm;
            profile.WeightKg = dto.WeightKg;
            profile.Allergies = dto.Allergies;
            profile.ChronicConditions = dto.ChronicConditions;
            profile.EmergencyContactName = dto.EmergencyContactName;
            profile.EmergencyContactPhone = dto.EmergencyContactPhone;

            await _repository.UpdateAsync(profile);
            await _repository.SaveChangesAsync();
        }
    }
}