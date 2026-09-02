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

        public async Task<List<DoctorProfile>> GetAllAsync(string? search = null, string? specialty = null, string? status = null)
        {
            var query = _context.DoctorProfiles
                .Include(d => d.User)
                .Include(d => d.Clinic)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim();

                query = query.Where(d =>
                    d.User.FullName.Contains(keyword) ||
                    d.User.Email.Contains(keyword) ||
                    (d.User.Phone != null && d.User.Phone.Contains(keyword)) ||
                    d.LicenseNumber.Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(specialty))
            {
                var value = specialty.Trim();
                query = query.Where(d => d.Specialty == value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var value = status.Trim().ToLower();
                query = query.Where(d => d.User.Status == value);
            }

            return await query.OrderBy(d => d.User.FullName).ToListAsync();
        }

        public async Task<List<DoctorProfile>> GetBookableAsync()
        {
            return await _context.DoctorProfiles
                .Include(d => d.User)
                .Include(d => d.Clinic)
                .Where(d => d.LicenseVerified == true && d.User.Status == "active")
                .OrderBy(d => d.User.FullName)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<DoctorProfile?> GetByIdAsync(Guid id)
        {
            return await _context.DoctorProfiles
                .Include(d => d.User)
                .Include(d => d.Clinic)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<DoctorProfile?> GetByUserIdAsync(Guid userId)
        {
            return await _context.DoctorProfiles
                .Include(d => d.User)
                .Include(d => d.Clinic)
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }

        public async Task<DoctorProfile?> GetByLicenseNumberAsync(string licenseNumber)
        {
            return await _context.DoctorProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.LicenseNumber == licenseNumber);
        }

        public async Task<bool> ClinicExistsAsync(Guid clinicId)
        {
            return await _context.Clinics.AnyAsync(c => c.Id == clinicId);
        }

        public async Task<List<Clinic>> GetClinicsAsync()
        {
            return await _context.Clinics
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<List<string>> GetSpecialtiesAsync()
        {
            return await _context.DoctorProfiles
                .AsNoTracking()
                .Select(d => d.Specialty)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();
        }

        public async Task AddAsync(DoctorProfile doctorProfile)
        {
            await _context.DoctorProfiles.AddAsync(doctorProfile);
        }

        public Task UpdateAsync(DoctorProfile doctorProfile)
        {
            _context.DoctorProfiles.Update(doctorProfile);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}