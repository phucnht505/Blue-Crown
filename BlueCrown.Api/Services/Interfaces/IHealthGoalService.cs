using BlueCrown.Api.DTOs.HealthGoals;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IHealthGoalService
    {
        Task<List<HealthGoalDto>> GetMyGoalsAsync(Guid patientId);
        Task<HealthGoalDto?> GetByIdAsync(Guid id, Guid patientId);
        Task<HealthGoalDto> CreateAsync(Guid patientId, CreateHealthGoalDto dto);
        Task<bool> UpdateAsync(Guid id, Guid patientId, UpdateHealthGoalDto dto);
        Task<bool> DeleteAsync(Guid id, Guid patientId);
    }
}