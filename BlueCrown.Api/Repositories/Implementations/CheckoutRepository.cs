using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class CheckoutRepository : ICheckoutRepository
    {
        private readonly BlueCrownContext _context;

        public CheckoutRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task AddAsync(EcommerceOrder order)
        {
            await _context.EcommerceOrders.AddAsync(order);
        }

        public async Task<EcommerceOrder?> GetByIdAsync(Guid id)
        {
            return await _context.EcommerceOrders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}