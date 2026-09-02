using BlueCrown.Api.DTOs.HealthGoals;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IHealthGoalService
    {
        Task<List<HealthGoalDto>> GetMyGoalsAsync(Guid userId);
        Task<HealthGoalDto?> GetByIdAsync(Guid id, Guid userId);
        Task<HealthGoalDto> CreateAsync(Guid userId, CreateHealthGoalDto dto);
        Task<bool> UpdateAsync(Guid id, Guid userId, UpdateHealthGoalDto dto);
        Task<bool> DeleteAsync(Guid id, Guid userId);

        Task<List<DoctorHealthGoalPatientDto>> GetDoctorPatientsAsync(Guid userId);
        Task<List<DoctorHealthGoalMetricTypeDto>> GetDoctorMetricTypesAsync(Guid userId);
        Task<List<HealthGoalDto>> GetDoctorPatientGoalsAsync(Guid userId, Guid patientId);
        Task<HealthGoalDto> CreateForPatientAsync(Guid userId, Guid patientId, CreateHealthGoalDto dto);
        Task<bool> UpdateForPatientAsync(Guid id, Guid userId, Guid patientId, UpdateHealthGoalDto dto);
        Task<bool> CancelForPatientAsync(Guid id, Guid userId, Guid patientId);
    }
}