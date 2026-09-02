using BlueCrown.Api.DTOs.MedicalRecords;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IMedicalRecordService
    {
        Task<List<MedicalRecordDto>> GetPatientRecordsAsync(Guid userId);
        Task<List<MedicalRecordDto>> GetDoctorRecordsAsync(Guid userId);
        Task<MedicalRecordDto?> GetPatientRecordByIdAsync(Guid id, Guid userId);
        Task<MedicalRecordDto?> GetDoctorRecordByIdAsync(Guid id, Guid userId);
        Task<MedicalRecordDto?> GetDoctorRecordByAppointmentAsync(Guid appointmentId, Guid userId);
        Task<MedicalRecordDto> CreateAsync(Guid userId, CreateMedicalRecordDto dto);
        Task<MedicalRecordDto?> UpdateAsync(Guid id, Guid userId, UpdateMedicalRecordDto dto);
    }
}