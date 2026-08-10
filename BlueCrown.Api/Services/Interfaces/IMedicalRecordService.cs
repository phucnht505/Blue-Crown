using BlueCrown.Api.DTOs.MedicalRecords;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IMedicalRecordService
    {
        Task<List<MedicalRecordDto>> GetAllAsync();

        Task<MedicalRecordDto?> GetByIdAsync(Guid id);

        Task<List<MedicalRecordDto>> GetByPatientIdAsync(Guid patientId);

        Task<List<MedicalRecordDto>> GetByDoctorIdAsync(Guid doctorId);

        Task<MedicalRecordDto?> GetByAppointmentIdAsync(Guid appointmentId);

        Task<MedicalRecordDto> CreateAsync(CreateMedicalRecordDto dto);

        Task<MedicalRecordDto?> UpdateAsync(Guid id, CreateMedicalRecordDto dto);

        Task<bool> DeleteAsync(Guid id);
    }
}