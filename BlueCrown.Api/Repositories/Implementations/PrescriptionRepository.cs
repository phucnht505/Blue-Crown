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
            return await BuildQuery()
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Prescription?> GetByIdAsync(Guid id)
        {
            return await BuildQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Prescription?> GetByIdForUpdateAsync(Guid id)
        {
            return await BuildQuery()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Prescription>> GetByPatientIdAsync(Guid patientId)
        {
            return await BuildQuery()
                .Where(p => p.PatientId == patientId)
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Prescription>> GetByDoctorIdAsync(Guid doctorId)
        {
            return await BuildQuery()
                .Where(p => p.DoctorId == doctorId)
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Prescription?> GetByAppointmentIdAsync(Guid appointmentId)
        {
            return await BuildQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.AppointmentId == appointmentId);
        }

        public async Task<Prescription?> GetByMedicalRecordIdAsync(Guid medicalRecordId)
        {
            return await BuildQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.MedicalRecordId == medicalRecordId);
        }

        public async Task AddAsync(Prescription prescription)
        {
            await _context.Prescriptions.AddAsync(prescription);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        private IQueryable<Prescription> BuildQuery()
        {
            return _context.Prescriptions
                .Include(p => p.Patient)
                    .ThenInclude(p => p.User)
                .Include(p => p.Doctor)
                    .ThenInclude(d => d.User)
                .Include(p => p.Appointment)
                .Include(p => p.MedicalRecord)
                    .ThenInclude(m => m.Appointment)
                .Include(p => p.PrescriptionItems)
                    .ThenInclude(i => i.Medication)
                .Include(p => p.PrescriptionItems)
                    .ThenInclude(i => i.PrescriptionDispenseItem)
                        .ThenInclude(d => d.Product)
                .Include(p => p.PrescriptionItems)
                    .ThenInclude(i => i.PrescriptionDispenseItem)
                        .ThenInclude(d => d.DispensedByNavigation)
                .AsSplitQuery();
        }
    }
}