using BlueCrown.Api.DTOs.Products;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMedicationRepository _medicationRepository;

        public ProductService(IProductRepository productRepository, IMedicationRepository medicationRepository)
        {
            _productRepository = productRepository;
            _medicationRepository = medicationRepository;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(MapToDto);
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product == null ? null : MapToDto(product);
        }

        public async Task<IEnumerable<ProductDto>> GetByMedicationIdAsync(Guid medicationId)
        {
            var medication = await _medicationRepository.GetByIdAsync(medicationId);

            // BR-PRO-MED-002: Medication phải tồn tại.
            if (medication == null)
                throw new ArgumentException("Không tìm thấy Medication.");

            var products = await _productRepository.GetByMedicationIdAsync(medicationId);
            return products.Select(MapToDto);
        }

        public async Task<IEnumerable<ProductDto>> SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new ArgumentException("Vui lòng nhập từ khóa tìm kiếm.");

            var value = keyword.Trim();

            if (value.Length > 100)
                throw new ArgumentException("Từ khóa tìm kiếm tối đa 100 ký tự.");

            var products = await _productRepository.SearchAsync(value);
            return products.Select(MapToDto);
        }

        public async Task CreateAsync(CreateProductDto dto)
        {
            ValidateProduct(dto.Name, dto.Price);
            await ValidateMedicationAsync(dto.MedicationId);

            var name = dto.Name.Trim();
            var duplicate = await _productRepository.GetByNameAsync(name);

            // BR-PRO-001: Không tạo trùng Product.
            if (duplicate != null)
                throw new InvalidOperationException("Product này đã tồn tại.");

            var product = new Product
            {
                Id = Guid.NewGuid(),
                MedicationId = dto.MedicationId,
                Name = name,
                Description = NormalizeOptional(dto.Description),
                Price = dto.Price,
                StockQuantity = 0,
                IsPrescriptionRequired = dto.IsPrescriptionRequired ?? false,
                ActiveIngredient = NormalizeOptional(dto.ActiveIngredient),
                TherapeuticGroup = NormalizeOptional(dto.TherapeuticGroup),
                DosageForm = NormalizeOptional(dto.DosageForm),
                Strength = NormalizeOptional(dto.Strength),
                ImageUrl = NormalizeOptional(dto.ImageUrl)
            };

            // BR-PRO-002: Product mới luôn bắt đầu stock = 0, tồn kho chỉ tăng qua phiếu nhập.
            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateProductDto dto)
        {
            ValidateProduct(dto.Name, dto.Price);

            var product = await _productRepository.GetByIdForUpdateAsync(id);

            if (product == null)
                return false;

            await ValidateMedicationAsync(dto.MedicationId);

            var name = dto.Name.Trim();
            var duplicate = await _productRepository.GetByNameAsync(name);

            // BR-PRO-001: Không được trùng tên Product khác.
            if (duplicate != null && duplicate.Id != id)
                throw new InvalidOperationException("Product này đã tồn tại.");

            product.MedicationId = dto.MedicationId;
            product.Name = name;
            product.Description = NormalizeOptional(dto.Description);
            product.Price = dto.Price;
            product.IsPrescriptionRequired = dto.IsPrescriptionRequired ?? false;
            product.ActiveIngredient = NormalizeOptional(dto.ActiveIngredient);
            product.TherapeuticGroup = NormalizeOptional(dto.TherapeuticGroup);
            product.DosageForm = NormalizeOptional(dto.DosageForm);
            product.Strength = NormalizeOptional(dto.Strength);
            product.ImageUrl = NormalizeOptional(dto.ImageUrl);

            // BR-PRO-003: Không cập nhật StockQuantity qua chức năng sửa Product.
            await _productRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _productRepository.GetByIdForUpdateAsync(id);

            if (product == null)
                return false;

            // BR-PRO-004: Không xóa Product vẫn còn tồn kho.
            if ((product.StockQuantity ?? 0) > 0)
                throw new InvalidOperationException("Không thể xóa Product vẫn còn tồn kho.");

            // BR-PRO-005: Không xóa Product đã phát sinh lịch sử nghiệp vụ.
            if (await _productRepository.HasReferencesAsync(id))
                throw new InvalidOperationException("Không thể xóa Product đã phát sinh đơn hàng, nhập kho, cấp thuốc hoặc dữ liệu liên quan.");

            await _productRepository.DeleteAsync(product);
            await _productRepository.SaveChangesAsync();

            return true;
        }

        private async Task ValidateMedicationAsync(Guid? medicationId)
        {
            if (!medicationId.HasValue)
                return;

            if (medicationId.Value == Guid.Empty)
                throw new ArgumentException("Medication không hợp lệ.");

            var medication = await _medicationRepository.GetByIdAsync(medicationId.Value);

            // BR-PRO-MED-002: MedicationId nếu có phải tồn tại.
            if (medication == null)
                throw new ArgumentException("Medication được chọn không tồn tại.");
        }

        private static void ValidateProduct(string name, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên Product không được để trống.");

            var normalizedName = name.Trim();

            // BR-PRO-006: Tên Product phải có ít nhất một chữ cái.
            if (!normalizedName.Any(char.IsLetter))
                throw new ArgumentException("Tên Product phải chứa ít nhất một chữ cái.");

            if (normalizedName.Length < 2 || normalizedName.Length > 100)
                throw new ArgumentException("Tên Product phải từ 2 đến 100 ký tự.");

            if (price <= 0)
                throw new ArgumentException("Giá bán phải lớn hơn 0.");
        }

        private static string? NormalizeOptional(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                MedicationId = product.MedicationId,
                MedicationName = product.Medication?.Name,
                MedicationGenericName = product.Medication?.GenericName,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                IsPrescriptionRequired = product.IsPrescriptionRequired,
                ActiveIngredient = product.ActiveIngredient,
                TherapeuticGroup = product.TherapeuticGroup,
                DosageForm = product.DosageForm,
                Strength = product.Strength,
                ImageUrl = product.ImageUrl
            };
        }
    }
}