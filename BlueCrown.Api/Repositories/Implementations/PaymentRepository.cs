using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly BlueCrownContext _context;

        public PaymentRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .ToListAsync();
        }

        public async Task<Payment?> GetByIdAsync(Guid id)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Payment>> GetByAppointmentIdAsync(Guid appointmentId)
        {
            return await _context.Payments
                .Where(p => p.AppointmentId == appointmentId)
                .ToListAsync();
        }

        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }

        public async Task UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}