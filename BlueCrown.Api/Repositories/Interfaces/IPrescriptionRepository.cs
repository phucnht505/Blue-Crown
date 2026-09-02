using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IPrescriptionRepository
    {
        Task<List<Prescription>> GetAllAsync();
        Task<Prescription?> GetByIdAsync(Guid id);
        Task<Prescription?> GetByIdForUpdateAsync(Guid id);
        Task<List<Prescription>> GetByPatientIdAsync(Guid patientId);
        Task<List<Prescription>> GetByDoctorIdAsync(Guid doctorId);
        Task<Prescription?> GetByAppointmentIdAsync(Guid appointmentId);
        Task<Prescription?> GetByMedicalRecordIdAsync(Guid medicalRecordId);
        Task AddAsync(Prescription prescription);
        Task SaveChangesAsync();
    }
}