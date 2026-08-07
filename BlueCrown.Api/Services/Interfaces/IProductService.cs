using BlueCrown.Api.DTOs.Products;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<ProductDto>> SearchAsync(string keyword);
        Task CreateAsync(CreateProductDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateProductDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
