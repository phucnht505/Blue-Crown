using BlueCrown.Api.DTOs.AutoPrescriptions;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IAutoPrescriptionService
    {
        Task<List<AutoPrescriptionDto>> GetAllAsync();
        Task<AutoPrescriptionDto?> GetByIdAsync(Guid id);
        Task<AutoPrescriptionDto?> GetByDiseaseNameAsync(string diseaseName);
        Task<AutoPrescriptionDto> AddAsync(CreateAutoPrescriptionDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateAutoPrescriptionDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}