using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Appointment?> GetByIdAsync(Guid id);
        Task<List<Appointment>> GetByPatientIdAsync(Guid patientId);
        Task<List<Appointment>> GetByDoctorIdAsync(Guid doctorId);
        Task<bool> HasDoctorPatientAccessAsync(Guid doctorId, Guid patientId);
        Task<bool> HasDoctorScheduleConflictAsync(Guid doctorId, DateTime scheduledAt);
        Task<bool> HasPatientScheduleConflictAsync(Guid patientId, DateTime scheduledAt);
        Task AddAsync(Appointment appointment);
        Task DeleteAsync(Appointment appointment);
        Task SaveChangesAsync();
    }
}