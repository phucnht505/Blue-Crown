using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IMedicalRecordRepository
    {
        Task<List<MedicalRecord>> GetAllAsync();

        Task<MedicalRecord?> GetByIdAsync(Guid id);

        Task<List<MedicalRecord>> GetByPatientIdAsync(Guid patientId);

        Task<List<MedicalRecord>> GetByDoctorIdAsync(Guid doctorId);

        Task<MedicalRecord?> GetByAppointmentIdAsync(Guid appointmentId);

        Task AddAsync(MedicalRecord medicalRecord);

        Task UpdateAsync(MedicalRecord medicalRecord);

        Task DeleteAsync(MedicalRecord medicalRecord);

        Task SaveChangesAsync();
    }
}