using BlueCrown.Api.DTOs.Appointments;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<List<AppointmentDto>> GetAllAsync();

        Task<AppointmentDto?> GetByIdAsync(Guid id);

        Task<List<AppointmentDto>> GetByPatientIdAsync(Guid patientId);

        Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto);

        Task<bool> DeleteAsync(Guid id);
    }
}