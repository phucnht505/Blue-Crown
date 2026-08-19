using BlueCrown.Api.DTOs.Clinics;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IClinicService
    {
        Task<List<ClinicDto>> GetAllAsync();
        Task<ClinicDto?> GetByIdAsync(Guid id);
        Task<ClinicDto> CreateAsync(CreateClinicDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateClinicDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}