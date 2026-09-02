using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly BlueCrownContext _context;

        public MedicalRecordRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task<MedicalRecord?> GetByIdAsync(Guid id)
        {
            return await _context.MedicalRecords
                .Include(x => x.Patient)
                    .ThenInclude(p => p.User)
                .Include(x => x.Doctor)
                    .ThenInclude(d => d.User)
                .Include(x => x.Appointment)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<MedicalRecord>> GetByPatientIdAsync(Guid patientId)
        {
            return await _context.MedicalRecords
                .Include(x => x.Patient)
                    .ThenInclude(p => p.User)
                .Include(x => x.Doctor)
                    .ThenInclude(d => d.User)
                .Include(x => x.Appointment)
                .Where(x => x.PatientId == patientId)
                .OrderByDescending(x => x.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<MedicalRecord>> GetByDoctorIdAsync(Guid doctorId)
        {
            return await _context.MedicalRecords
                .Include(x => x.Patient)
                    .ThenInclude(p => p.User)
                .Include(x => x.Doctor)
                    .ThenInclude(d => d.User)
                .Include(x => x.Appointment)
                .Where(x => x.DoctorId == doctorId)
                .OrderByDescending(x => x.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<MedicalRecord?> GetByAppointmentIdAsync(Guid appointmentId)
        {
            return await _context.MedicalRecords
                .Include(x => x.Patient)
                    .ThenInclude(p => p.User)
                .Include(x => x.Doctor)
                    .ThenInclude(d => d.User)
                .Include(x => x.Appointment)
                .FirstOrDefaultAsync(x => x.AppointmentId == appointmentId);
        }

        public async Task AddAsync(MedicalRecord medicalRecord)
        {
            await _context.MedicalRecords.AddAsync(medicalRecord);
        }

        public Task UpdateAsync(MedicalRecord medicalRecord)
        {
            _context.MedicalRecords.Update(medicalRecord);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}