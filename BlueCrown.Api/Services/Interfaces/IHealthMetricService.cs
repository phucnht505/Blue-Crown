using BlueCrown.Api.DTOs.HealthMetrics;
using BlueCrown.Api.DTOs.MetricTypes;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IHealthMetricService
    {
        Task<List<MetricTypeDto>> GetMetricTypesAsync();

        Task<List<HealthMetricDto>> GetMyMetricsAsync(Guid userId);

        Task<HealthMetricDto?> GetByIdAsync(Guid id, Guid userId);

        Task<HealthMetricDto?> GetLatestAsync(Guid userId);

        Task<HealthMetricDto> CreateAsync(Guid userId, CreateHealthMetricDto dto);
    }
}