using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class EcommerceOrderRepository : IEcommerceOrderRepository
    {
        private readonly BlueCrownContext _context;

        public EcommerceOrderRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task<List<EcommerceOrder>> GetAllAsync()
        {
            return await _context.EcommerceOrders
                .Include(x => x.OrderItems)
                .ToListAsync();
        }

        public async Task<EcommerceOrder?> GetByIdAsync(Guid id)
        {
            return await _context.EcommerceOrders
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(EcommerceOrder order)
        {
            await _context.EcommerceOrders.AddAsync(order);
        }

        public async Task UpdateAsync(EcommerceOrder order)
        {
            _context.EcommerceOrders.Update(order);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(EcommerceOrder order)
        {
            _context.EcommerceOrders.Remove(order);
            await Task.CompletedTask;
        }

        public async Task<bool> UserExistsAsync(Guid userId)
        {
            return await _context.Users
                .AnyAsync(x => x.Id == userId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}