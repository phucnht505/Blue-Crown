using BlueCrown.Api.DTOs.EcommerceOrders;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IEcommerceOrderService
    {
        Task<List<EcommerceOrderDto>> GetAllAsync();

        Task<EcommerceOrderDto?> GetByIdAsync(Guid id);

        Task<EcommerceOrderDto> CreateAsync(
            CreateEcommerceOrderDto dto);

        Task<bool> DeleteAsync(Guid id);
    }
}