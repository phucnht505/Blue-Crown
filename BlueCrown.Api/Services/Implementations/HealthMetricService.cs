using BlueCrown.Api.DTOs.HealthMetrics;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class HealthMetricService : IHealthMetricService
    {
        private readonly IHealthMetricRepository _repository;

        public HealthMetricService(IHealthMetricRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<HealthMetricDto>> GetMyMetricsAsync(Guid patientId)
        {
            var metrics = await _repository.GetByPatientIdAsync(patientId);
            return metrics.Select(MapToDto).ToList();
        }

        public async Task<HealthMetricDto?> GetByIdAsync(Guid id, Guid patientId)
        {
            var metric = await _repository.GetByIdAsync(id);

            if (metric == null || metric.PatientId != patientId)
                return null;

            return MapToDto(metric);
        }

        public async Task<HealthMetricDto?> GetLatestAsync(Guid patientId)
        {
            var metric = await _repository.GetLatestAsync(patientId);
            return metric == null ? null : MapToDto(metric);
        }

        public async Task<HealthMetricDto> CreateAsync(Guid patientId, CreateHealthMetricDto dto)
        {
            var metricType = await _repository.GetMetricTypeAsync(dto.MetricTypeId);

            if (metricType == null)
                throw new ArgumentException("MetricType không tồn tại.");

            if (dto.Value < 0)
                throw new ArgumentException("Giá trị chỉ số không được âm.");

            if (metricType.NormalMin.HasValue && metricType.NormalMax.HasValue &&
                metricType.NormalMin > metricType.NormalMax)
                throw new InvalidOperationException("Khoảng giá trị bình thường của MetricType không hợp lệ.");

            var metric = new HealthMetric
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                MetricTypeId = dto.MetricTypeId,
                Value = dto.Value,
                RecordedAt = dto.RecordedAt ?? DateTime.UtcNow
            };

            await _repository.AddAsync(metric);
            await _repository.SaveChangesAsync();

            var created = await _repository.GetByIdAsync(metric.Id);

            if (created == null)
                throw new Exception("Không thể lấy HealthMetric vừa tạo.");

            return MapToDto(created);
        }

        private static HealthMetricDto MapToDto(HealthMetric metric)
        {
            return new HealthMetricDto
            {
                Id = metric.Id,
                PatientId = metric.PatientId,
                MetricTypeId = metric.MetricTypeId,
                MetricTypeCode = metric.MetricType.Code,
                MetricTypeName = metric.MetricType.Name,
                MetricTypeUnit = metric.MetricType.Unit,
                Value = metric.Value,
                RecordedAt = metric.RecordedAt,
                NormalMin = metric.MetricType.NormalMin,
                NormalMax = metric.MetricType.NormalMax
            };
        }
    }
}