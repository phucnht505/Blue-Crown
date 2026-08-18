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
            return await _context.Medications
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<Medication?> GetByIdAsync(Guid id)
        {
            return await _context.Medications
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(Medication medication)
        {
            await _context.Medications.AddAsync(medication);
        }

        public async Task UpdateAsync(Medication medication)
        {
            _context.Medications.Update(medication);

            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}