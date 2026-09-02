using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IHealthGoalRepository
    {
        Task<List<HealthGoal>> GetByPatientIdAsync(Guid patientId);
        Task<HealthGoal?> GetByIdAsync(Guid id);
        Task<List<MetricType>> GetMetricTypesAsync();
        Task AddAsync(HealthGoal healthGoal);
        Task UpdateAsync(HealthGoal healthGoal);
        Task DeleteAsync(HealthGoal healthGoal);
        Task<bool> MetricTypeExistsAsync(int metricTypeId);
        Task SaveChangesAsync();
    }
}