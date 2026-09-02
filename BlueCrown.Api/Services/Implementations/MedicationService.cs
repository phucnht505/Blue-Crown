using BlueCrown.Api.DTOs.Medications;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class MedicationService : IMedicationService
    {
        private readonly IMedicationRepository _repository;

        public MedicationService(IMedicationRepository repository)
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
            return medication == null ? null : MapToDto(medication);
        }

        public async Task<MedicationDto> CreateAsync(CreateMedicationDto dto)
        {
            ValidateMedication(dto.Name, dto.GenericName, dto.Category);

            var name = dto.Name.Trim();
            var duplicate = await _repository.GetByNameAsync(name);

            // BR-MED-001: Không tạo trùng tên Medication.
            if (duplicate != null)
                throw new InvalidOperationException("Medication này đã tồn tại.");

            var medication = new Medication
            {
                Id = Guid.NewGuid(),
                Name = name,
                GenericName = NormalizeOptional(dto.GenericName),
                Category = NormalizeOptional(dto.Category)
            };

            await _repository.AddAsync(medication);
            await _repository.SaveChangesAsync();

            return MapToDto(medication);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateMedicationDto dto)
        {
            ValidateMedication(dto.Name, dto.GenericName, dto.Category);

            var medication = await _repository.GetByIdForUpdateAsync(id);

            if (medication == null)
                return false;

            var name = dto.Name.Trim();
            var duplicate = await _repository.GetByNameAsync(name);

            // BR-MED-001: Không được trùng tên Medication khác.
            if (duplicate != null && duplicate.Id != id)
                throw new InvalidOperationException("Medication này đã tồn tại.");

            medication.Name = name;
            medication.GenericName = NormalizeOptional(dto.GenericName);
            medication.Category = NormalizeOptional(dto.Category);

            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var medication = await _repository.GetByIdForUpdateAsync(id);

            if (medication == null)
                return false;

            // BR-MED-002: Không xóa Medication đang được Product hoặc Prescription sử dụng.
            if (await _repository.HasUsageAsync(id))
                throw new InvalidOperationException("Không thể xóa Medication đang được sử dụng bởi Product hoặc đơn thuốc.");

            await _repository.DeleteAsync(medication);
            await _repository.SaveChangesAsync();

            return true;
        }

        private static void ValidateMedication(string name, string? genericName, string? category)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên Medication không được để trống.");

            var normalizedName = name.Trim();

            // BR-MED-003: Tên Medication phải chứa ít nhất một chữ cái.
            if (!normalizedName.Any(char.IsLetter))
                throw new ArgumentException("Tên Medication phải chứa ít nhất một chữ cái.");

            if (normalizedName.Length < 2 || normalizedName.Length > 150)
                throw new ArgumentException("Tên Medication phải từ 2 đến 150 ký tự.");

            if (!string.IsNullOrWhiteSpace(genericName) && !genericName.Trim().Any(char.IsLetter))
                throw new ArgumentException("Tên generic phải chứa ít nhất một chữ cái.");

            if (!string.IsNullOrWhiteSpace(category) && !category.Trim().Any(char.IsLetter))
                throw new ArgumentException("Nhóm thuốc phải chứa ít nhất một chữ cái.");
        }

        private static string? NormalizeOptional(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
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