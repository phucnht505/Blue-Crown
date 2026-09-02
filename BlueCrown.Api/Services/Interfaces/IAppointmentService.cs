using BlueCrown.Api.DTOs.Appointments;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<List<AppointmentDto>> GetMyAppointmentsAsync(Guid userId);
        Task<List<AppointmentDoctorDto>> GetBookableDoctorsAsync();
        Task<AppointmentDto?> GetByIdAsync(Guid id, Guid userId);
        Task<AppointmentDto> CreateAsync(Guid userId, CreateAppointmentDto dto);
        Task<bool> DeleteAsync(Guid id, Guid userId);

        Task<List<AppointmentDto>> GetDoctorAppointmentsAsync(Guid userId);
        Task<AppointmentDto?> UpdateDoctorStatusAsync(Guid id, Guid userId, UpdateAppointmentStatusDto dto);
    }
}