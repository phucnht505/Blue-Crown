using BlueCrown.Api.DTOs.HealthMetrics;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IHealthMetricService
    {
        Task<List<HealthMetricDto>> GetMyMetricsAsync(Guid patientId);
        Task<HealthMetricDto?> GetByIdAsync(Guid id, Guid patientId);
        Task<HealthMetricDto?> GetLatestAsync(Guid patientId);
        Task<HealthMetricDto> CreateAsync(Guid patientId, CreateHealthMetricDto dto);
    }
}