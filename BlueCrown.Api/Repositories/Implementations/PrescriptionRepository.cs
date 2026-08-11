using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly BlueCrownContext _context;

        public PrescriptionRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task<List<Prescription>> GetAllAsync()
        {
            return await _context.Prescriptions
                .Include(p => p.PrescriptionItems)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Prescription?> GetByIdAsync(Guid id)
        {
            return await _context.Prescriptions
                .Include(p => p.PrescriptionItems)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Prescription> AddAsync(Prescription prescription)
        {
            await _context.Prescriptions.AddAsync(prescription);

            return prescription;
        }

        public async Task UpdateAsync(Prescription prescription)
        {
            _context.Prescriptions.Update(prescription);

            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Prescription prescription)
        {
            _context.Prescriptions.Remove(prescription);

            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}