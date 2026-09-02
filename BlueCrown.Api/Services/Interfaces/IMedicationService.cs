using BlueCrown.Api.DTOs.Medications;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IMedicationService
    {
        Task<List<MedicationDto>> GetAllAsync();
        Task<MedicationDto?> GetByIdAsync(Guid id);
        Task<MedicationDto> CreateAsync(CreateMedicationDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateMedicationDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}