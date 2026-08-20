using BlueCrown.Api.DTOs.MetricTypes;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IMetricTypeService
    {
        Task<List<MetricTypeDto>> GetAllAsync();
        Task<MetricTypeDto?> GetByIdAsync(int id);
        Task<MetricTypeDto> CreateAsync(CreateMetricTypeDto dto);
        Task<bool> UpdateAsync(int id, UpdateMetricTypeDto dto);
        Task<bool> DeleteAsync(int id);
    }
}