using BlueCrown.Api.DTOs.Checkout;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface ICheckoutService
    {
        Task<CheckoutResponseDto> CreateAsync(CreateCheckoutDto dto, Guid? userId);
        Task<CheckoutResponseDto?> GetByIdAsync(Guid id);
    }
}