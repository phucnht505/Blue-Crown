using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class ClinicRepository : IClinicRepository
    {
        private readonly BlueCrownContext _context;

        public ClinicRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task<List<Clinic>> GetAllAsync()
        {
            return await _context.Clinics.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<Clinic?> GetByIdAsync(Guid id)
        {
            return await _context.Clinics.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(Clinic clinic)
        {
            await _context.Clinics.AddAsync(clinic);
        }

        public async Task UpdateAsync(Clinic clinic)
        {
            _context.Clinics.Update(clinic);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Clinic clinic)
        {
            _context.Clinics.Remove(clinic);
            await Task.CompletedTask;
        }

        public async Task<bool> HasDoctorsAsync(Guid clinicId)
        {
            return await _context.DoctorProfiles.AnyAsync(x => x.ClinicId == clinicId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}