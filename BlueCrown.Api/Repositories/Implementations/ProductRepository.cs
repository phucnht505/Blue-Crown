using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly BlueCrownContext _context;

        public ProductRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.Include(p => p.Medication).OrderBy(p => p.Name).AsNoTracking().ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products.Include(p => p.Medication).AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product?> GetByNameAsync(string name)
        {
            var normalizedName = name.Trim().ToLower();
            return await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Name.ToLower() == normalizedName);
        }

        public async Task<IEnumerable<Product>> GetByMedicationIdAsync(Guid medicationId)
        {
            return await _context.Products.Include(p => p.Medication).Where(p => p.MedicationId == medicationId).OrderBy(p => p.Name).AsNoTracking().ToListAsync();
        }

        public async Task<List<Product>> GetByIdsForUpdateAsync(List<Guid> ids)
        {
            return await _context.Products.Where(p => ids.Contains(p.Id)).ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchAsync(string keyword)
        {
            var value = keyword.Trim();

            return await _context.Products
                .Include(p => p.Medication)
                .Where(p => p.Name.Contains(value) ||
                    (p.ActiveIngredient != null && p.ActiveIngredient.Contains(value)) ||
                    (p.Medication != null && p.Medication.Name.Contains(value)) ||
                    (p.Medication != null && p.Medication.GenericName != null && p.Medication.GenericName.Contains(value)))
                .OrderBy(p => p.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> HasReferencesAsync(Guid id)
        {
            if (await _context.OrderItems.AnyAsync(x => x.ProductId == id))
                return true;

            if (await _context.ReceiptDetails.AnyAsync(x => x.ProductId == id))
                return true;

            if (await _context.PrescriptionDispenseItems.AnyAsync(x => x.ProductId == id))
                return true;

            if (await _context.DrugAlternatives.AnyAsync(x => x.ProductId == id || x.AlternativeProductId == id))
                return true;

            if (await _context.AutoPrescriptions.AnyAsync(x => x.RecommendedProductId == id))
                return true;

            return false;
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}