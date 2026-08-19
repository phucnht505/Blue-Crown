using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IHealthMetricRepository
    {
        Task<List<HealthMetric>> GetByPatientIdAsync(Guid patientId);
        Task<HealthMetric?> GetByIdAsync(Guid id);
        Task<HealthMetric?> GetLatestAsync(Guid patientId);
        Task<bool> MetricTypeExistsAsync(int metricTypeId);
        Task<MetricType?> GetMetricTypeAsync(int metricTypeId);
        Task AddAsync(HealthMetric healthMetric);
        Task SaveChangesAsync();
    }
}