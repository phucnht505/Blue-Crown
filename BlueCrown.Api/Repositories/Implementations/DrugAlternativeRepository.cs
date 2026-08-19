using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class DrugAlternativeRepository : IDrugAlternativeRepository
    {
        private readonly BlueCrownContext _context;

        public DrugAlternativeRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task<List<DrugAlternative>> GetAllAsync()
        {
            return await _context.DrugAlternatives
                .Include(x => x.Product)
                .Include(x => x.AlternativeProduct)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<DrugAlternative?> GetByIdAsync(Guid id)
        {
            return await _context.DrugAlternatives
                .Include(x => x.Product)
                .Include(x => x.AlternativeProduct)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<DrugAlternative>> GetByProductIdAsync(Guid productId)
        {
            return await _context.DrugAlternatives
                .Include(x => x.Product)
                .Include(x => x.AlternativeProduct)
                .Where(x => x.ProductId == productId)
                .OrderByDescending(x => x.SimilarityScore)
                .ToListAsync();
        }

        public async Task<bool> ProductExistsAsync(Guid productId)
        {
            return await _context.Products.AnyAsync(x => x.Id == productId);
        }

        public async Task<bool> ExistsAsync(Guid productId, Guid alternativeProductId, Guid? excludeId = null)
        {
            return await _context.DrugAlternatives.AnyAsync(x =>
                x.ProductId == productId &&
                x.AlternativeProductId == alternativeProductId &&
                (!excludeId.HasValue || x.Id != excludeId.Value));
        }

        public async Task AddAsync(DrugAlternative drugAlternative)
        {
            await _context.DrugAlternatives.AddAsync(drugAlternative);
        }

        public Task UpdateAsync(DrugAlternative drugAlternative)
        {
            _context.DrugAlternatives.Update(drugAlternative);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(DrugAlternative drugAlternative)
        {
            _context.DrugAlternatives.Remove(drugAlternative);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}