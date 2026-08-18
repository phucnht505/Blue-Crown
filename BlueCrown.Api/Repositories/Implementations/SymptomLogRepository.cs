using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class SymptomLogRepository : ISymptomLogRepository
    {
        private readonly BlueCrownContext _context;

        public SymptomLogRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task<List<SymptomLog>> GetByPatientIdAsync(
            Guid patientId)
        {
            return await _context.SymptomLogs
                .AsNoTracking()
                .Where(x => x.PatientId == patientId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<SymptomLog?> GetByIdAsync(Guid id)
        {
            return await _context.SymptomLogs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<SymptomLog?> GetLatestByPatientIdAsync(
            Guid patientId)
        {
            return await _context.SymptomLogs
                .AsNoTracking()
                .Where(x => x.PatientId == patientId)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(SymptomLog symptomLog)
        {
            await _context.SymptomLogs.AddAsync(symptomLog);
        }

        public async Task UpdateAsync(SymptomLog symptomLog)
        {
            _context.SymptomLogs.Update(symptomLog);

            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}