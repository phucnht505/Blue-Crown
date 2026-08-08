using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class ReceiptDetailRepository : IReceiptDetailRepository
    {
        private readonly BlueCrownContext _context;

        public ReceiptDetailRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task<List<ReceiptDetail>> GetFefoDetailsAsync(
            Guid productId)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            return await _context.ReceiptDetails
                .Where(x =>
                    x.ProductId == productId &&
                    x.ExpirationDate >= today)
                .OrderBy(x => x.ExpirationDate)
                .ThenBy(x => x.BatchNumber)
                .ToListAsync();
        }
    }
}