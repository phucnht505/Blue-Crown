using BlueCrown.Api.DTOs.Prescriptions;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _repository;

        public PrescriptionService(IPrescriptionRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PrescriptionDto>> GetAllAsync()
        {
            var prescriptions = await _repository.GetAllAsync();

            return prescriptions.Select(MapToDto).ToList();
        }

        public async Task<PrescriptionDto?> GetByIdAsync(Guid id)
        {
            var prescription = await _repository.GetByIdAsync(id);

            if (prescription == null)
                return null;

            return MapToDto(prescription);
        }

        public async Task<PrescriptionDto> CreateAsync(CreatePrescriptionDto dto)
        {
            // Tạo Prescription
            var prescription = new Prescription
            {
                Id = Guid.NewGuid(),
                MedicalRecordId = dto.MedicalRecordId,
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                Status = dto.Status ?? "Pending",
                CreatedAt = DateTime.UtcNow
            };

            // Tạo các PrescriptionItem
            prescription.PrescriptionItems = dto.Items.Select(item => new PrescriptionItem
            {
                Id = Guid.NewGuid(),

                // Quan trọng:
                // Gán PrescriptionId bằng Id của Prescription vừa tạo
                PrescriptionId = prescription.Id,

                MedicationId = item.MedicationId,
                Dosage = item.Dosage,
                FrequencyPerDay = item.FrequencyPerDay,
                DurationDays = item.DurationDays,
                Instructions = item.Instructions
            }).ToList();

            await _repository.AddAsync(prescription);

            await _repository.SaveChangesAsync();

            return MapToDto(prescription);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdatePrescriptionDto dto)
        {
            var prescription = await _repository.GetByIdAsync(id);

            if (prescription == null)
                return false;

            prescription.Status = dto.Status;

            await _repository.UpdateAsync(prescription);

            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var prescription = await _repository.GetByIdAsync(id);

            if (prescription == null)
                return false;

            await _repository.DeleteAsync(prescription);

            await _repository.SaveChangesAsync();

            return true;
        }

        private static PrescriptionDto MapToDto(Prescription prescription)
        {
            return new PrescriptionDto
            {
                Id = prescription.Id,
                MedicalRecordId = prescription.MedicalRecordId,
                PatientId = prescription.PatientId,
                DoctorId = prescription.DoctorId,
                Status = prescription.Status,
                CreatedAt = prescription.CreatedAt,

                Items = prescription.PrescriptionItems
                    .Select(item => new PrescriptionItemDto
                    {
                        Id = item.Id,

                        // Lấy đúng PrescriptionId
                        PrescriptionId = item.PrescriptionId,

                        MedicationId = item.MedicationId,
                        Dosage = item.Dosage,
                        FrequencyPerDay = item.FrequencyPerDay,
                        DurationDays = item.DurationDays,
                        Instructions = item.Instructions
                    })
                    .ToList()
            };
        }
    }
}