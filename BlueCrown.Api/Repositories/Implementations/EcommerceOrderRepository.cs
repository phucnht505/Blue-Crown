using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

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
            return await BuildQuery().OrderByDescending(x => x.CreatedAt).AsNoTracking().ToListAsync();
        }

        public async Task<EcommerceOrder?> GetByIdAsync(Guid id)
        {
            return await BuildQuery().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<EcommerceOrder?> GetByIdForUpdateAsync(Guid id)
        {
            return await BuildQuery().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<EcommerceOrder>> GetByUserIdAsync(Guid userId)
        {
            return await BuildQuery().Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedAt).AsNoTracking().ToListAsync();
        }

        public async Task<List<EcommerceOrder>> GetGuestOrdersByPhoneAsync(string guestPhone)
        {
            return await BuildQuery()
                .Where(x => x.UserId == null && x.GuestPhone == guestPhone)
                .OrderByDescending(x => x.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> HasActiveOrderByPrescriptionIdAsync(Guid prescriptionId)
        {
            return await _context.EcommerceOrders.AnyAsync(x => x.PrescriptionId == prescriptionId && x.OrderStatus != "cancelled");
        }

        public async Task AddAsync(EcommerceOrder order)
        {
            await _context.EcommerceOrders.AddAsync(order);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginSerializableTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        }

        private IQueryable<EcommerceOrder> BuildQuery()
        {
            return _context.EcommerceOrders
                .Include(x => x.User)
                .Include(x => x.Prescription)
                .Include(x => x.OrderItems).ThenInclude(x => x.Product);
        }
    }
}