using BlueCrown.Api.DTOs.Suppliers;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<List<SupplierDto>> GetAllAsync();
        Task<SupplierDto?> GetByIdAsync(Guid id);
        Task<SupplierDto> CreateAsync(CreateSupplierDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateSupplierDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}