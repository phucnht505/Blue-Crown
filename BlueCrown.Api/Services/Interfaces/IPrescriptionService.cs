using BlueCrown.Api.DTOs.Prescriptions;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IPrescriptionService
    {
        Task<List<PrescriptionDto>> GetAllAsync();

        Task<PrescriptionDto?> GetByIdAsync(Guid id);

        Task<PrescriptionDto> CreateAsync(CreatePrescriptionDto dto);

        Task<bool> UpdateAsync(Guid id, UpdatePrescriptionDto dto);

        Task<bool> DeleteAsync(Guid id);
    }
}