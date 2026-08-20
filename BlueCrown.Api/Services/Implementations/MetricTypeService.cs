using BlueCrown.Api.DTOs.MetricTypes;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class MetricTypeService : IMetricTypeService
    {
        private readonly IMetricTypeRepository _repository;

        public MetricTypeService(IMetricTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MetricTypeDto>> GetAllAsync()
        {
            var metricTypes = await _repository.GetAllAsync();

            return metricTypes.Select(MapToDto).ToList();
        }

        public async Task<MetricTypeDto?> GetByIdAsync(int id)
        {
            var metricType = await _repository.GetByIdAsync(id);

            if (metricType == null)
                return null;

            return MapToDto(metricType);
        }

        public async Task<MetricTypeDto> CreateAsync(CreateMetricTypeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new ArgumentException("Code không được để trống.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Tên MetricType không được để trống.");

            if (string.IsNullOrWhiteSpace(dto.Unit))
                throw new ArgumentException("Unit không được để trống.");

            if (dto.NormalMin.HasValue && dto.NormalMax.HasValue && dto.NormalMin > dto.NormalMax)
                throw new ArgumentException("NormalMin không được lớn hơn NormalMax.");

            var code = dto.Code.Trim().ToUpper();

            var existing = await _repository.GetByCodeAsync(code);

            if (existing != null)
                throw new InvalidOperationException("Code MetricType đã tồn tại.");

            var metricType = new MetricType
            {
                Code = code,
                Name = dto.Name.Trim(),
                Unit = dto.Unit.Trim(),
                NormalMin = dto.NormalMin,
                NormalMax = dto.NormalMax
            };

            await _repository.AddAsync(metricType);
            await _repository.SaveChangesAsync();

            return MapToDto(metricType);
        }

        public async Task<bool> UpdateAsync(int id, UpdateMetricTypeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new ArgumentException("Code không được để trống.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Tên MetricType không được để trống.");

            if (string.IsNullOrWhiteSpace(dto.Unit))
                throw new ArgumentException("Unit không được để trống.");

            if (dto.NormalMin.HasValue && dto.NormalMax.HasValue && dto.NormalMin > dto.NormalMax)
                throw new ArgumentException("NormalMin không được lớn hơn NormalMax.");

            var metricType = await _repository.GetByIdAsync(id);

            if (metricType == null)
                return false;

            var code = dto.Code.Trim().ToUpper();

            var existing = await _repository.GetByCodeAsync(code);

            if (existing != null && existing.Id != id)
                throw new InvalidOperationException("Code MetricType đã tồn tại.");

            metricType.Code = code;
            metricType.Name = dto.Name.Trim();
            metricType.Unit = dto.Unit.Trim();
            metricType.NormalMin = dto.NormalMin;
            metricType.NormalMax = dto.NormalMax;

            await _repository.UpdateAsync(metricType);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var metricType = await _repository.GetByIdAsync(id);

            if (metricType == null)
                return false;

            var hasHealthGoals = await _repository.HasHealthGoalsAsync(id);
            var hasHealthMetrics = await _repository.HasHealthMetricsAsync(id);

            if (hasHealthGoals || hasHealthMetrics)
                throw new InvalidOperationException("Không thể xóa MetricType đang được sử dụng.");

            await _repository.DeleteAsync(metricType);
            await _repository.SaveChangesAsync();

            return true;
        }

        private static MetricTypeDto MapToDto(MetricType metricType)
        {
            return new MetricTypeDto
            {
                Id = metricType.Id,
                Code = metricType.Code,
                Name = metricType.Name,
                Unit = metricType.Unit,
                NormalMin = metricType.NormalMin,
                NormalMax = metricType.NormalMax
            };
        }
    }
}