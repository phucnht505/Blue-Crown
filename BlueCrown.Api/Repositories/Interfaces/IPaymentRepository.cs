using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetAllAsync();

        Task<Payment?> GetByIdAsync(Guid id);

        Task<IEnumerable<Payment>> GetByAppointmentIdAsync(Guid appointmentId);

        Task AddAsync(Payment payment);

        Task UpdateAsync(Payment payment);

        Task SaveChangesAsync();
    }
}