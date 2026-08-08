using BlueCrown.Api.DTOs.Payments;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDto>> GetAllAsync();

        Task<PaymentDto?> GetByIdAsync(Guid id);

        Task<IEnumerable<PaymentDto>> GetByAppointmentIdAsync(Guid appointmentId);

        Task<PaymentDto> CreateAsync(CreatePaymentDto dto);

        Task<bool> UpdateStatusAsync(Guid id, string status);
    }
}