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
            ValidateSupplier(dto.SupplierName, dto.ContactPhone);

            var supplierName = dto.SupplierName.Trim();
            var contactPhone = dto.ContactPhone.Trim();
            var existing = await _repository.GetByNameAsync(supplierName);

            // BR-SUP-001: Không được trùng tên nhà cung cấp.
            if (existing != null)
                throw new InvalidOperationException("Nhà cung cấp này đã tồn tại.");

            var supplier = new Supplier
            {
                Id = Guid.NewGuid(),
                SupplierName = supplierName,
                ContactPhone = contactPhone,
                GdpCertified = dto.GdpCertified,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(supplier);
            await _repository.SaveChangesAsync();

            return MapToDto(supplier);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateSupplierDto dto)
        {
            ValidateSupplier(dto.SupplierName, dto.ContactPhone);

            var supplier = await _repository.GetByIdAsync(id);

            if (supplier == null)
                return false;

            var supplierName = dto.SupplierName.Trim();
            var contactPhone = dto.ContactPhone.Trim();
            var existing = await _repository.GetByNameAsync(supplierName);

            // BR-SUP-001: Không được trùng tên nhà cung cấp.
            if (existing != null && existing.Id != id)
                throw new InvalidOperationException("Nhà cung cấp này đã tồn tại.");

            supplier.SupplierName = supplierName;
            supplier.ContactPhone = contactPhone;
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

            // BR-SUP-002: Không xóa Supplier đã phát sinh phiếu nhập.
            if (supplier.InventoryReceipts.Any())
                throw new InvalidOperationException("Không thể xóa nhà cung cấp đã có phiếu nhập.");

            await _repository.DeleteAsync(supplier);
            await _repository.SaveChangesAsync();

            return true;
        }

        private static void ValidateSupplier(string supplierName, string contactPhone)
        {
            if (string.IsNullOrWhiteSpace(supplierName))
                throw new ArgumentException("Tên nhà cung cấp không được để trống.");

            supplierName = supplierName.Trim();

            // BR-SUP-003: Tên nhà cung cấp phải có ít nhất một chữ cái.
            if (!supplierName.Any(char.IsLetter))
                throw new ArgumentException("Tên nhà cung cấp không được chỉ chứa số hoặc ký tự đặc biệt.");

            if (supplierName.Length < 2 || supplierName.Length > 255)
                throw new ArgumentException("Tên nhà cung cấp phải từ 2 đến 255 ký tự.");

            // BR-SUP-004: Số điện thoại là bắt buộc.
            if (string.IsNullOrWhiteSpace(contactPhone))
                throw new ArgumentException("Số điện thoại không được để trống.");

            contactPhone = contactPhone.Trim();

            // BR-SUP-005: Số điện thoại phải là số di động Việt Nam hợp lệ.
            if (!System.Text.RegularExpressions.Regex.IsMatch(contactPhone, @"^(0[35789]\d{8}|\+84[35789]\d{8})$"))
                throw new ArgumentException("Số điện thoại không hợp lệ.");
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