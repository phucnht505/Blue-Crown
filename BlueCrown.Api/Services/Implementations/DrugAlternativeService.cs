using BlueCrown.Api.DTOs.DrugAlternatives;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class DrugAlternativeService : IDrugAlternativeService
    {
        private readonly IDrugAlternativeRepository _repository;

        public DrugAlternativeService(IDrugAlternativeRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DrugAlternativeDto>> GetAllAsync()
        {
            var alternatives = await _repository.GetAllAsync();
            return alternatives.Select(MapToDto).ToList();
        }

        public async Task<DrugAlternativeDto?> GetByIdAsync(Guid id)
        {
            var alternative = await _repository.GetByIdAsync(id);
            return alternative == null ? null : MapToDto(alternative);
        }

        public async Task<List<DrugAlternativeDto>> GetByProductIdAsync(Guid productId)
        {
            if (!await _repository.ProductExistsAsync(productId))
                throw new ArgumentException("Product không tồn tại.");

            var alternatives = await _repository.GetByProductIdAsync(productId);
            return alternatives.Select(MapToDto).ToList();
        }

        public async Task<DrugAlternativeDto> CreateAsync(CreateDrugAlternativeDto dto)
        {
            Validate(dto.ProductId, dto.AlternativeProductId, dto.SimilarityScore);

            if (!await _repository.ProductExistsAsync(dto.ProductId))
                throw new ArgumentException("Product không tồn tại.");

            if (!await _repository.ProductExistsAsync(dto.AlternativeProductId))
                throw new ArgumentException("AlternativeProduct không tồn tại.");

            if (await _repository.ExistsAsync(dto.ProductId, dto.AlternativeProductId))
                throw new InvalidOperationException("Thuốc thay thế này đã tồn tại.");

            var alternative = new DrugAlternative
            {
                Id = Guid.NewGuid(),
                ProductId = dto.ProductId,
                AlternativeProductId = dto.AlternativeProductId,
                Reason = string.IsNullOrWhiteSpace(dto.Reason) ? null : dto.Reason.Trim(),
                SimilarityScore = dto.SimilarityScore,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(alternative);
            await _repository.SaveChangesAsync();

            var created = await _repository.GetByIdAsync(alternative.Id);

            if (created == null)
                throw new Exception("Không thể lấy DrugAlternative vừa tạo.");

            return MapToDto(created);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateDrugAlternativeDto dto)
        {
            Validate(dto.ProductId, dto.AlternativeProductId, dto.SimilarityScore);

            if (!await _repository.ProductExistsAsync(dto.ProductId))
                throw new ArgumentException("Product không tồn tại.");

            if (!await _repository.ProductExistsAsync(dto.AlternativeProductId))
                throw new ArgumentException("AlternativeProduct không tồn tại.");

            var alternative = await _repository.GetByIdAsync(id);

            if (alternative == null)
                return false;

            if (await _repository.ExistsAsync(dto.ProductId, dto.AlternativeProductId, id))
                throw new InvalidOperationException("Thuốc thay thế này đã tồn tại.");

            alternative.ProductId = dto.ProductId;
            alternative.AlternativeProductId = dto.AlternativeProductId;
            alternative.Reason = string.IsNullOrWhiteSpace(dto.Reason) ? null : dto.Reason.Trim();
            alternative.SimilarityScore = dto.SimilarityScore;

            await _repository.UpdateAsync(alternative);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var alternative = await _repository.GetByIdAsync(id);

            if (alternative == null)
                return false;

            await _repository.DeleteAsync(alternative);
            await _repository.SaveChangesAsync();

            return true;
        }

        private static void Validate(Guid productId, Guid alternativeProductId, decimal? similarityScore)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId không hợp lệ.");

            if (alternativeProductId == Guid.Empty)
                throw new ArgumentException("AlternativeProductId không hợp lệ.");

            if (productId == alternativeProductId)
                throw new ArgumentException("Product không thể là AlternativeProduct của chính nó.");

            if (similarityScore.HasValue && similarityScore < 0)
                throw new ArgumentException("SimilarityScore không được âm.");
        }

        private static DrugAlternativeDto MapToDto(DrugAlternative x)
        {
            return new DrugAlternativeDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                AlternativeProductId = x.AlternativeProductId,
                AlternativeProductName = x.AlternativeProduct.Name,
                Reason = x.Reason,
                SimilarityScore = x.SimilarityScore,
                CreatedAt = x.CreatedAt
            };
        }
    }
}