using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class AutoPrescriptionRepository : IAutoPrescriptionRepository
    {
        private readonly BlueCrownContext _context;

        public AutoPrescriptionRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task<List<AutoPrescription>> GetAllAsync()
        {
            return await _context.AutoPrescriptions
                .AsNoTracking()
                .Include(x => x.RecommendedProduct)
                .OrderBy(x => x.DiseaseName)
                .ToListAsync();
        }

        public async Task<AutoPrescription?> GetByIdAsync(Guid id)
        {
            return await _context.AutoPrescriptions
                .AsNoTracking()
                .Include(x => x.RecommendedProduct)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<AutoPrescription?> GetByDiseaseNameAsync(string diseaseName)
        {
            var name = diseaseName.Trim().ToLower();

            return await _context.AutoPrescriptions
                .AsNoTracking()
                .Include(x => x.RecommendedProduct)
                .FirstOrDefaultAsync(x => x.DiseaseName.ToLower() == name);
        }

        public async Task AddAsync(AutoPrescription autoPrescription)
        {
            await _context.AutoPrescriptions.AddAsync(autoPrescription);
        }

        public async Task UpdateAsync(AutoPrescription autoPrescription)
        {
            _context.AutoPrescriptions.Update(autoPrescription);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(AutoPrescription autoPrescription)
        {
            _context.AutoPrescriptions.Remove(autoPrescription);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}