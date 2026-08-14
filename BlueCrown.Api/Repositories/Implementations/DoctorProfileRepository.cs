using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class DoctorProfileRepository : IDoctorProfileRepository
    {
        private readonly BlueCrownContext _context;

        public DoctorProfileRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task<List<DoctorProfile>> GetAllAsync()
        {
            return await _context.DoctorProfiles
                .Include(d => d.Clinic)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<DoctorProfile?> GetByIdAsync(Guid id)
        {
            return await _context.DoctorProfiles
                .Include(d => d.Clinic)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<DoctorProfile?> GetByUserIdAsync(Guid userId)
        {
            return await _context.DoctorProfiles
                .Include(d => d.Clinic)
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }

        public async Task AddAsync(DoctorProfile doctorProfile)
        {
            await _context.DoctorProfiles.AddAsync(doctorProfile);
        }

        public async Task UpdateAsync(DoctorProfile doctorProfile)
        {
            _context.DoctorProfiles.Update(doctorProfile);

            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}