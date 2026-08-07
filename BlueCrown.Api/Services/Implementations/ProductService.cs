using BlueCrown.Api.DTOs.Products;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();

            return products.Select(MapToDto);
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                return null;

            return MapToDto(product);
        }

        public async Task<IEnumerable<ProductDto>> SearchAsync(string keyword)
        {
            var products = await _productRepository.SearchAsync(keyword);

            return products.Select(MapToDto);
        }

        public async Task CreateAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                IsPrescriptionRequired = dto.IsPrescriptionRequired,
                ActiveIngredient = dto.ActiveIngredient,
                TherapeuticGroup = dto.TherapeuticGroup,
                DosageForm = dto.DosageForm,
                Strength = dto.Strength
            };

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                return false;

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.IsPrescriptionRequired = dto.IsPrescriptionRequired;
            product.ActiveIngredient = dto.ActiveIngredient;
            product.TherapeuticGroup = dto.TherapeuticGroup;
            product.DosageForm = dto.DosageForm;
            product.Strength = dto.Strength;

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                return false;

            await _productRepository.DeleteAsync(product);
            await _productRepository.SaveChangesAsync();

            return true;
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                IsPrescriptionRequired = product.IsPrescriptionRequired,
                ActiveIngredient = product.ActiveIngredient,
                TherapeuticGroup = product.TherapeuticGroup,
                DosageForm = product.DosageForm,
                Strength = product.Strength
            };
        }
    }
}