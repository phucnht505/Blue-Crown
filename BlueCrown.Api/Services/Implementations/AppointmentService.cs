using BlueCrown.Api.DTOs.Appointments;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<List<AppointmentDto>> GetAllAsync()
        {
            var appointments = await _appointmentRepository.GetAllAsync();

            return appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                DoctorId = a.DoctorId,
                ScheduledAt = a.ScheduledAt,
                Type = a.Type,
                Status = a.Status,
                CreatedAt = a.CreatedAt
            }).ToList();
        }

        public async Task<AppointmentDto?> GetByIdAsync(Guid id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            if (appointment == null)
            {
                return null;
            }

            return new AppointmentDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                ScheduledAt = appointment.ScheduledAt,
                Type = appointment.Type,
                Status = appointment.Status,
                CreatedAt = appointment.CreatedAt
            };
        }

        public async Task<List<AppointmentDto>> GetByPatientIdAsync(Guid patientId)
        {
            var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId);

            return appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                DoctorId = a.DoctorId,
                ScheduledAt = a.ScheduledAt,
                Type = a.Type,
                Status = a.Status,
                CreatedAt = a.CreatedAt
            }).ToList();
        }

        public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto)
        {
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),

                PatientId = dto.PatientId,

                DoctorId = dto.DoctorId,

                ScheduledAt = dto.ScheduledAt,

                Type = dto.Type,

                Status = "Pending",

                CreatedAt = DateTime.UtcNow
            };

            await _appointmentRepository.AddAsync(appointment);

            await _appointmentRepository.SaveChangesAsync();

            return new AppointmentDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                ScheduledAt = appointment.ScheduledAt,
                Type = appointment.Type,
                Status = appointment.Status,
                CreatedAt = appointment.CreatedAt
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var deleted = await _appointmentRepository.DeleteAsync(id);

            if (!deleted)
            {
                return false;
            }

            await _appointmentRepository.SaveChangesAsync();

            return true;
        }
    }
}