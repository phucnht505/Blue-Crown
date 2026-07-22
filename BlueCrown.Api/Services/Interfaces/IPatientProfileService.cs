using BlueCrown.Api.DTOs.PatientProfiles;
namespace BlueCrown.Api.Services.Interfaces
{
    public interface IPatientProfileService
    {
        Task<PatientProfileDto?> GetMyProfileAsync(Guid userId);
        Task CreateProfileAsync(Guid userId, CreatePatientProfileDto dto);
        Task UpdateProfileAsync(Guid userId, UpdatePatientProfileDto dto);
    }
}
