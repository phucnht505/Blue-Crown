using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class MedicationRepository : IMedicationRepository
    {
        private readonly BlueCrownContext _context;

        public MedicationRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task<List<Medication>> GetAllAsync()
        {
            return await _context.Medications.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<Medication?> GetByIdAsync(Guid id)
        {
            return await _context.Medications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Medication?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.Medications.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Medication?> GetByNameAsync(string name)
        {
            var normalizedName = name.Trim().ToLower();
            return await _context.Medications.AsNoTracking().FirstOrDefaultAsync(x => x.Name.ToLower() == normalizedName);
        }

        public async Task<List<Medication>> GetByIdsAsync(List<Guid> ids)
        {
            return await _context.Medications.AsNoTracking().Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        public async Task<bool> HasUsageAsync(Guid id)
        {
            return await _context.Products.AnyAsync(x => x.MedicationId == id) || await _context.PrescriptionItems.AnyAsync(x => x.MedicationId == id);
        }

        public async Task AddAsync(Medication medication)
        {
            await _context.Medications.AddAsync(medication);
        }

        public Task UpdateAsync(Medication medication)
        {
            _context.Medications.Update(medication);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Medication medication)
        {
            _context.Medications.Remove(medication);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}