using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IMetricTypeRepository
    {
        Task<List<MetricType>> GetAllAsync();
        Task<MetricType?> GetByIdAsync(int id);
        Task<MetricType?> GetByCodeAsync(string code);
        Task<bool> HasHealthGoalsAsync(int metricTypeId);
        Task<bool> HasHealthMetricsAsync(int metricTypeId);
        Task AddAsync(MetricType metricType);
        Task UpdateAsync(MetricType metricType);
        Task DeleteAsync(MetricType metricType);
        Task SaveChangesAsync();
    }
}