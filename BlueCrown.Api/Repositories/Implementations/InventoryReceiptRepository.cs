using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class InventoryReceiptRepository : IInventoryReceiptRepository
    {
        private readonly BlueCrownContext _context;

        public InventoryReceiptRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InventoryReceipt>> GetAllAsync()
        {
            return await BuildQuery().OrderByDescending(r => r.ReceiptDate).ToListAsync();
        }

        public async Task<InventoryReceipt?> GetByIdAsync(Guid id)
        {
            return await BuildQuery().FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(InventoryReceipt receipt)
        {
            await _context.InventoryReceipts.AddAsync(receipt);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        private IQueryable<InventoryReceipt> BuildQuery()
        {
            return _context.InventoryReceipts
                .Include(r => r.Supplier)
                .Include(r => r.CreatedByNavigation)
                .Include(r => r.ApprovedByNavigation)
                .Include(r => r.ReceiptDetails).ThenInclude(d => d.Product);
        }
    }
}