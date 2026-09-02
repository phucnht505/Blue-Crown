using BlueCrown.Api.DTOs.Prescriptions;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IPrescriptionService
    {
        Task<List<PrescriptionDto>> GetPatientPrescriptionsAsync(Guid userId);
        Task<PrescriptionDto?> GetPatientPrescriptionByIdAsync(Guid id, Guid userId);

        Task<List<PrescriptionDto>> GetDoctorPrescriptionsAsync(Guid userId);
        Task<PrescriptionDto?> GetDoctorPrescriptionByIdAsync(Guid id, Guid userId);
        Task<PrescriptionDto?> GetDoctorPrescriptionByMedicalRecordAsync(Guid medicalRecordId, Guid userId);
        Task<PrescriptionDto> CreateAsync(Guid userId, CreatePrescriptionDto dto);

        Task<List<PrescriptionDto>> GetPharmacistPrescriptionsAsync();
        Task<PrescriptionDto?> GetPharmacistPrescriptionByIdAsync(Guid id);
        Task<PrescriptionDto?> UpdatePharmacistStatusAsync(Guid id, UpdatePrescriptionStatusDto dto);
        Task<PrescriptionDto?> DispenseAsync(Guid id, Guid pharmacistUserId, DispensePrescriptionDto dto);
    }
}