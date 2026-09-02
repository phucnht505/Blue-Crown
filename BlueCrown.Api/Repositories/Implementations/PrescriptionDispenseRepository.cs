using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class PrescriptionDispenseRepository : IPrescriptionDispenseRepository
    {
        private readonly BlueCrownContext _context;

        public PrescriptionDispenseRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<PrescriptionDispenseItem> items)
        {
            await _context.PrescriptionDispenseItems.AddRangeAsync(items);
        }
    }
}