using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetAllAsync();

        Task<Appointment?> GetByIdAsync(Guid id);

        Task<List<Appointment>> GetByPatientIdAsync(Guid patientId);

        Task<Appointment> AddAsync(Appointment appointment);

        Task<bool> DeleteAsync(Guid id);

        Task SaveChangesAsync();
    }
}