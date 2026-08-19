using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly BlueCrownContext _context;

        public SupplierRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task<List<Supplier>> GetAllAsync()
        {
            return await _context.Suppliers
                .OrderBy(x => x.SupplierName)
                .ToListAsync();
        }

        public async Task<Supplier?> GetByIdAsync(Guid id)
        {
            return await _context.Suppliers
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Supplier?> GetByNameAsync(string supplierName)
        {
            return await _context.Suppliers
                .FirstOrDefaultAsync(x => x.SupplierName == supplierName);
        }

        public async Task AddAsync(Supplier supplier)
        {
            await _context.Suppliers.AddAsync(supplier);
        }

        public Task UpdateAsync(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Supplier supplier)
        {
            _context.Suppliers.Remove(supplier);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}