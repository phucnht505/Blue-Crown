using BlueCrown.Api.DTOs.DrugAlternatives;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IDrugAlternativeService
    {
        Task<List<DrugAlternativeDto>> GetAllAsync();
        Task<DrugAlternativeDto?> GetByIdAsync(Guid id);
        Task<List<DrugAlternativeDto>> GetByProductIdAsync(Guid productId);
        Task<DrugAlternativeDto> CreateAsync(CreateDrugAlternativeDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateDrugAlternativeDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}