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

        public async Task<List<MedicalRecord>> GetAllAsync()
        {
            return await _context.MedicalRecords
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<MedicalRecord?> GetByIdAsync(Guid id)
        {
            return await _context.MedicalRecords
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<MedicalRecord>> GetByPatientIdAsync(Guid patientId)
        {
            return await _context.MedicalRecords
                .AsNoTracking()
                .Where(x => x.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<List<MedicalRecord>> GetByDoctorIdAsync(Guid doctorId)
        {
            return await _context.MedicalRecords
                .AsNoTracking()
                .Where(x => x.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task<MedicalRecord?> GetByAppointmentIdAsync(Guid appointmentId)
        {
            return await _context.MedicalRecords
                .FirstOrDefaultAsync(x => x.AppointmentId == appointmentId);
        }

        public async Task AddAsync(MedicalRecord medicalRecord)
        {
            await _context.MedicalRecords.AddAsync(medicalRecord);
        }

        public async Task UpdateAsync(MedicalRecord medicalRecord)
        {
            _context.MedicalRecords.Update(medicalRecord);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(MedicalRecord medicalRecord)
        {
            _context.MedicalRecords.Remove(medicalRecord);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}