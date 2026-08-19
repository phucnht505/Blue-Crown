using BlueCrown.Api.DTOs.AutoPrescriptions;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class AutoPrescriptionService : IAutoPrescriptionService
    {
        private readonly IAutoPrescriptionRepository _repository;

        public AutoPrescriptionService(IAutoPrescriptionRepository repository)
        {
            _repository = repository;
        }

        // GET ALL
        public async Task<List<AutoPrescriptionDto>> GetAllAsync()
        {
            var prescriptions = await _repository.GetAllAsync();

            return prescriptions.Select(x => new AutoPrescriptionDto
            {
                Id = x.Id,
                DiseaseName = x.DiseaseName,
                RecommendedProductId = x.RecommendedProductId,
                DosageInstructions = x.DosageInstructions
            }).ToList();
        }

        // GET BY ID
        public async Task<AutoPrescriptionDto?> GetByIdAsync(Guid id)
        {
            var prescription = await _repository.GetByIdAsync(id);

            if (prescription == null)
                return null;

            return new AutoPrescriptionDto
            {
                Id = prescription.Id,
                DiseaseName = prescription.DiseaseName,
                RecommendedProductId = prescription.RecommendedProductId,
                DosageInstructions = prescription.DosageInstructions
            };
        }

        // GET BY DISEASE NAME
        public async Task<AutoPrescriptionDto?> GetByDiseaseNameAsync(string diseaseName)
        {
            if (string.IsNullOrWhiteSpace(diseaseName))
                return null;

            var prescription = await _repository.GetByDiseaseNameAsync(diseaseName);

            if (prescription == null)
                return null;

            return new AutoPrescriptionDto
            {
                Id = prescription.Id,
                DiseaseName = prescription.DiseaseName,
                RecommendedProductId = prescription.RecommendedProductId,
                DosageInstructions = prescription.DosageInstructions
            };
        }

        // CREATE
        public async Task<AutoPrescriptionDto> AddAsync(CreateAutoPrescriptionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.DiseaseName))
                throw new ArgumentException("Tên bệnh không được để trống.");

            var existing = await _repository.GetByDiseaseNameAsync(dto.DiseaseName);

            if (existing != null)
                throw new InvalidOperationException("Đã tồn tại đơn thuốc tự động cho bệnh này.");

            var prescription = new AutoPrescription
            {
                Id = Guid.NewGuid(),
                DiseaseName = dto.DiseaseName.Trim(),
                RecommendedProductId = dto.RecommendedProductId,
                DosageInstructions = dto.DosageInstructions?.Trim()
            };

            await _repository.AddAsync(prescription);
            await _repository.SaveChangesAsync();

            return new AutoPrescriptionDto
            {
                Id = prescription.Id,
                DiseaseName = prescription.DiseaseName,
                RecommendedProductId = prescription.RecommendedProductId,
                DosageInstructions = prescription.DosageInstructions
            };
        }

        // UPDATE
        public async Task<bool> UpdateAsync(Guid id, UpdateAutoPrescriptionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.DiseaseName))
                throw new ArgumentException("Tên bệnh không được để trống.");

            var prescription = await _repository.GetByIdAsync(id);

            if (prescription == null)
                return false;

            var existing = await _repository.GetByDiseaseNameAsync(dto.DiseaseName);

            if (existing != null && existing.Id != id)
                throw new InvalidOperationException("Đã tồn tại đơn thuốc tự động cho bệnh này.");

            prescription.DiseaseName = dto.DiseaseName.Trim();
            prescription.RecommendedProductId = dto.RecommendedProductId;
            prescription.DosageInstructions = dto.DosageInstructions?.Trim();

            await _repository.UpdateAsync(prescription);
            await _repository.SaveChangesAsync();

            return true;
        }

        // DELETE
        public async Task<bool> DeleteAsync(Guid id)
        {
            var prescription = await _repository.GetByIdAsync(id);

            if (prescription == null)
                return false;

            await _repository.DeleteAsync(prescription);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}