using BlueCrown.Api.DTOs.HealthMetrics;
using BlueCrown.Api.DTOs.MetricTypes;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class HealthMetricService : IHealthMetricService
    {
        private readonly IHealthMetricRepository _repository;
        private readonly IPatientProfileRepository _patientProfileRepository;

        public HealthMetricService(
            IHealthMetricRepository repository,
            IPatientProfileRepository patientProfileRepository)
        {
            _repository = repository;
            _patientProfileRepository = patientProfileRepository;
        }

        public async Task<List<MetricTypeDto>> GetMetricTypesAsync()
        {
            var metricTypes = await _repository.GetMetricTypesAsync();

            return metricTypes.Select(x => new MetricTypeDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Unit = x.Unit,
                NormalMin = x.NormalMin,
                NormalMax = x.NormalMax
            }).ToList();
        }

        public async Task<List<HealthMetricDto>> GetMyMetricsAsync(Guid userId)
        {
            var patientProfile = await GetPatientProfileAsync(userId);

            var metrics = await _repository.GetByPatientIdAsync(
                patientProfile.Id
            );

            return metrics.Select(MapToDto).ToList();
        }

        public async Task<HealthMetricDto?> GetByIdAsync(Guid id, Guid userId)
        {
            var patientProfile = await GetPatientProfileAsync(userId);

            var metric = await _repository.GetByIdAsync(id);

            // BR-HM-004: Patient chỉ được xem chỉ số sức khỏe của chính mình.
            if (metric == null || metric.PatientId != patientProfile.Id)
                return null;

            return MapToDto(metric);
        }

        public async Task<HealthMetricDto?> GetLatestAsync(Guid userId)
        {
            var patientProfile = await GetPatientProfileAsync(userId);

            var metric = await _repository.GetLatestAsync(
                patientProfile.Id
            );

            return metric == null
                ? null
                : MapToDto(metric);
        }

        public async Task<HealthMetricDto> CreateAsync(
            Guid userId,
            CreateHealthMetricDto dto)
        {
            var patientProfile = await GetPatientProfileAsync(userId);

            var metricType = await _repository.GetMetricTypeAsync(
                dto.MetricTypeId
            );

            // BR-HM-002: Loại chỉ số phải tồn tại.
            if (metricType == null)
                throw new ArgumentException(
                    "Loại chỉ số sức khỏe không tồn tại."
                );

            // BR-HM-003: Giá trị chỉ số không được âm.
            if (dto.Value < 0)
                throw new ArgumentException(
                    "Giá trị chỉ số không được âm."
                );

            if (metricType.NormalMin.HasValue &&
                metricType.NormalMax.HasValue &&
                metricType.NormalMin > metricType.NormalMax)
            {
                throw new InvalidOperationException(
                    "Khoảng giá trị bình thường của loại chỉ số không hợp lệ."
                );
            }

            var metric = new HealthMetric
            {
                Id = Guid.NewGuid(),

                // QUAN TRỌNG:
                // HealthMetric.PatientId = PatientProfile.Id
                // KHÔNG phải User.Id.
                PatientId = patientProfile.Id,

                MetricTypeId = dto.MetricTypeId,
                Value = dto.Value,
                RecordedAt = dto.RecordedAt ?? DateTime.UtcNow
            };

            await _repository.AddAsync(metric);
            await _repository.SaveChangesAsync();

            var created = await _repository.GetByIdAsync(metric.Id);

            if (created == null)
            {
                throw new Exception(
                    "Không thể lấy chỉ số sức khỏe vừa tạo."
                );
            }

            return MapToDto(created);
        }

        // BR-HM-001: User phải có PatientProfile trước khi quản lý HealthMetric.
        private async Task<PatientProfile> GetPatientProfileAsync(Guid userId)
        {
            var patientProfile =
                await _patientProfileRepository.GetByUserIdAsync(userId);

            if (patientProfile == null)
            {
                throw new InvalidOperationException(
                    "Bạn cần tạo hồ sơ sức khỏe trước khi sử dụng chức năng này."
                );
            }

            return patientProfile;
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