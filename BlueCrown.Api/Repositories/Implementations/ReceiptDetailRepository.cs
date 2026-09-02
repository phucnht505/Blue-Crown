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

        public async Task<List<ReceiptDetail>> GetFefoDetailsAsync(Guid productId)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            // BR-FEFO-001: Chỉ lô thuộc phiếu đã duyệt và chưa hết hạn mới được đưa vào danh sách FEFO.
            return await _context.ReceiptDetails
                .Include(x => x.Receipt)
                .Where(x =>
                    x.ProductId == productId &&
                    x.Receipt != null &&
                    x.Receipt.Status == "approved" &&
                    x.ExpirationDate > today)
                .OrderBy(x => x.ExpirationDate)
                .ThenBy(x => x.BatchNumber)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}