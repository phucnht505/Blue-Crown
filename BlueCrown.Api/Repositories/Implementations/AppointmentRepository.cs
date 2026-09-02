using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly BlueCrownContext _context;

        public AppointmentRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task<Appointment?> GetByIdAsync(Guid id)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Clinic)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Appointment>> GetByPatientIdAsync(Guid patientId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Clinic)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.ScheduledAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetByDoctorIdAsync(Guid doctorId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Clinic)
                .Where(a => a.DoctorId == doctorId)
                .OrderByDescending(a => a.ScheduledAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> HasDoctorPatientAccessAsync(Guid doctorId, Guid patientId)
        {
            // BR-HG-DOCTOR-001: Doctor chỉ quản lý mục tiêu của Patient có lịch confirmed hoặc completed với mình.
            return await _context.Appointments.AnyAsync(a =>
                a.DoctorId == doctorId &&
                a.PatientId == patientId &&
                (a.Status == "confirmed" || a.Status == "completed"));
        }

        public async Task<bool> HasDoctorScheduleConflictAsync(Guid doctorId, DateTime scheduledAt)
        {
            return await _context.Appointments.AnyAsync(a =>
                a.DoctorId == doctorId &&
                a.ScheduledAt == scheduledAt &&
                (a.Status == null || a.Status != "cancelled"));
        }

        public async Task<bool> HasPatientScheduleConflictAsync(Guid patientId, DateTime scheduledAt)
        {
            return await _context.Appointments.AnyAsync(a =>
                a.PatientId == patientId &&
                a.ScheduledAt == scheduledAt &&
                (a.Status == null || a.Status != "cancelled"));
        }

        public async Task AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
        }

        public Task DeleteAsync(Appointment appointment)
        {
            _context.Appointments.Remove(appointment);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}