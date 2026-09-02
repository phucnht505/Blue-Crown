using BlueCrown.Api.DTOs.EcommerceOrders;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IEcommerceOrderService
    {
        Task<List<EcommerceOrderDto>> GetManagementOrdersAsync();
        Task<EcommerceOrderDto?> GetManagementOrderByIdAsync(Guid id);
        Task<List<EcommerceOrderDto>> GetMyOrdersAsync(Guid userId);
        Task<EcommerceOrderDto?> GetMyOrderByIdAsync(Guid id, Guid userId);
        Task<List<EcommerceOrderDto>> LookupGuestOrdersAsync(GuestOrderLookupDto dto);
        Task<EcommerceOrderDto> CreateAsync(Guid? userId, CreateEcommerceOrderDto dto);
        Task<EcommerceOrderDto?> UpdateStatusAsync(Guid id, UpdateEcommerceOrderStatusDto dto);
        Task<EcommerceOrderDto?> CancelMyOrderAsync(Guid id, Guid userId);
    }
}