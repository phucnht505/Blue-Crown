using BlueCrown.Api.DTOs.Medications;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class MedicationService : IMedicationService
    {
        private readonly IMedicationRepository _repository;

        public MedicationService(
            IMedicationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MedicationDto>> GetAllAsync()
        {
            var medications = await _repository.GetAllAsync();

            return medications.Select(MapToDto).ToList();
        }

        public async Task<MedicationDto?> GetByIdAsync(Guid id)
        {
            var medication = await _repository.GetByIdAsync(id);

            if (medication == null)
            {
                return null;
            }

            return MapToDto(medication);
        }

        public async Task<MedicationDto> CreateAsync(CreateMedicationDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new Exception("Medication name cannot be empty.");
            }

            var name = dto.Name.Trim();

            // Không tạo trùng thuốc theo tên.
            var medications = await _repository.GetAllAsync();

            var duplicate = medications.Any(x => string.Equals(x.Name.Trim(), name, StringComparison.OrdinalIgnoreCase));

            if (duplicate)
            {
                throw new Exception("Medication with the same name already exists.");
            }

            var medication = new Medication
            {
                Id = Guid.NewGuid(),

                Name = name,

                GenericName = string.IsNullOrWhiteSpace(dto.GenericName) ? null : dto.GenericName.Trim(),

                Category = string.IsNullOrWhiteSpace(dto.Category) ? null : dto.Category.Trim()
            };

            await _repository.AddAsync(medication);

            await _repository.SaveChangesAsync();

            var createdMedication = await _repository.GetByIdAsync(medication.Id);

            if (createdMedication == null)
            {
                throw new Exception("Failed to retrieve created medication.");
            }

            return MapToDto(createdMedication);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateMedicationDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new Exception("Medication name cannot be empty.");
            }

            var medication = await _repository.GetByIdAsync(id);

            if (medication == null)
            {
                return false;
            }

            var name = dto.Name.Trim();

            var medications = await _repository.GetAllAsync();

            var duplicate = medications.Any(x =>x.Id != id && string.Equals(x.Name.Trim(), name, StringComparison.OrdinalIgnoreCase));

            if (duplicate)
            {
                throw new Exception("Medication with the same name already exists.");
            }

            medication.Name = name;

            medication.GenericName = string.IsNullOrWhiteSpace(dto.GenericName) ? null : dto.GenericName.Trim();

            medication.Category = string.IsNullOrWhiteSpace(dto.Category) ? null : dto.Category.Trim();

            await _repository.UpdateAsync(medication);

            await _repository.SaveChangesAsync();

            return true;
        }

        private static MedicationDto MapToDto(Medication medication)
        {
            return new MedicationDto
            {
                Id = medication.Id,

                Name = medication.Name,

                GenericName = medication.GenericName,

                Category = medication.Category
            };
        }
    }
}