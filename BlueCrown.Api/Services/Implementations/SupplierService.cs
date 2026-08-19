using BlueCrown.Api.DTOs.Suppliers;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _repository;

        public SupplierService(ISupplierRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SupplierDto>> GetAllAsync()
        {
            var suppliers = await _repository.GetAllAsync();

            return suppliers.Select(MapToDto).ToList();
        }

        public async Task<SupplierDto?> GetByIdAsync(Guid id)
        {
            var supplier = await _repository.GetByIdAsync(id);

            return supplier == null ? null : MapToDto(supplier);
        }

        public async Task<SupplierDto> CreateAsync(CreateSupplierDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SupplierName))
                throw new ArgumentException("Tên nhà cung cấp không được để trống.");

            var supplierName = dto.SupplierName.Trim();

            var existing = await _repository.GetByNameAsync(supplierName);

            if (existing != null)
                throw new InvalidOperationException("Nhà cung cấp này đã tồn tại.");

            var supplier = new Supplier
            {
                Id = Guid.NewGuid(),
                SupplierName = supplierName,
                ContactPhone = string.IsNullOrWhiteSpace(dto.ContactPhone) ? null : dto.ContactPhone.Trim(),
                GdpCertified = dto.GdpCertified,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(supplier);
            await _repository.SaveChangesAsync();

            return MapToDto(supplier);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateSupplierDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SupplierName))
                throw new ArgumentException("Tên nhà cung cấp không được để trống.");

            var supplier = await _repository.GetByIdAsync(id);

            if (supplier == null)
                return false;

            var supplierName = dto.SupplierName.Trim();

            var existing = await _repository.GetByNameAsync(supplierName);

            if (existing != null && existing.Id != id)
                throw new InvalidOperationException("Nhà cung cấp này đã tồn tại.");

            supplier.SupplierName = supplierName;
            supplier.ContactPhone = string.IsNullOrWhiteSpace(dto.ContactPhone) ? null : dto.ContactPhone.Trim();
            supplier.GdpCertified = dto.GdpCertified;

            await _repository.UpdateAsync(supplier);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var supplier = await _repository.GetByIdAsync(id);

            if (supplier == null)
                return false;

            if (supplier.InventoryReceipts.Any())
                throw new InvalidOperationException("Không thể xóa nhà cung cấp đã có phiếu nhập.");

            await _repository.DeleteAsync(supplier);
            await _repository.SaveChangesAsync();

            return true;
        }

        private static SupplierDto MapToDto(Supplier supplier)
        {
            return new SupplierDto
            {
                Id = supplier.Id,
                SupplierName = supplier.SupplierName,
                ContactPhone = supplier.ContactPhone,
                GdpCertified = supplier.GdpCertified,
                CreatedAt = supplier.CreatedAt
            };
        }
    }
}