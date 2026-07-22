using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace BlueCrown.Api.Repositories.Implementations
{
    public class PatientProfileRepository : IPatientProfileRepository
    {
        private readonly BlueCrownContext _context;
        public PatientProfileRepository(BlueCrownContext context)
        {
            _context = context;
        }
        public async Task<PatientProfile?> GetByUserIdAsync(Guid userId)
        {
            return await _context.PatientProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        }
        public async Task AddAsync(PatientProfile patientProfile)
        {
            await _context.PatientProfiles.AddAsync(patientProfile);
        }
        public Task UpdateAsync(PatientProfile patientProfile)
        {
            _context.PatientProfiles.Update(patientProfile);
            return Task.CompletedTask;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
