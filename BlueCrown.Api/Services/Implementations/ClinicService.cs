using BlueCrown.Api.DTOs.Clinics;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class ClinicService : IClinicService
    {
        private readonly IClinicRepository _repository;

        public ClinicService(IClinicRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ClinicDto>> GetAllAsync()
        {
            var clinics = await _repository.GetAllAsync();
            return clinics.Select(MapToDto).ToList();
        }

        public async Task<ClinicDto?> GetByIdAsync(Guid id)
        {
            var clinic = await _repository.GetByIdAsync(id);
            return clinic == null ? null : MapToDto(clinic);
        }

        public async Task<ClinicDto> CreateAsync(CreateClinicDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Tên phòng khám không được để trống.");

            var clinics = await _repository.GetAllAsync();
            var name = dto.Name.Trim();

            if (clinics.Any(x => string.Equals(x.Name.Trim(), name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Phòng khám với tên này đã tồn tại.");

            var clinic = new Clinic
            {
                Id = Guid.NewGuid(),
                Name = name,
                Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim(),
                Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim()
            };

            await _repository.AddAsync(clinic);
            await _repository.SaveChangesAsync();

            return MapToDto(clinic);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateClinicDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Tên phòng khám không được để trống.");

            var clinic = await _repository.GetByIdAsync(id);

            if (clinic == null)
                return false;

            var name = dto.Name.Trim();
            var clinics = await _repository.GetAllAsync();

            if (clinics.Any(x => x.Id != id && string.Equals(x.Name.Trim(), name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Phòng khám với tên này đã tồn tại.");

            clinic.Name = name;
            clinic.Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim();
            clinic.Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();

            await _repository.UpdateAsync(clinic);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var clinic = await _repository.GetByIdAsync(id);

            if (clinic == null)
                return false;

            if (await _repository.HasDoctorsAsync(id))
                throw new InvalidOperationException("Không thể xóa phòng khám đang có bác sĩ.");

            await _repository.DeleteAsync(clinic);
            await _repository.SaveChangesAsync();

            return true;
        }

        private static ClinicDto MapToDto(Clinic clinic)
        {
            return new ClinicDto
            {
                Id = clinic.Id,
                Name = clinic.Name,
                Address = clinic.Address,
                Phone = clinic.Phone
            };
        }
    }
}