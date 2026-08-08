using BlueCrown.Api.DTOs.InventoryReceipts;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class FefoService : IFefoService
    {
        private readonly IReceiptDetailRepository _receiptDetailRepository;

        public FefoService(
            IReceiptDetailRepository receiptDetailRepository)
        {
            _receiptDetailRepository = receiptDetailRepository;
        }

        public async Task<List<ReceiptDetailDto>> GetFefoAsync(Guid productId)
        {
            var details =
                await _receiptDetailRepository.GetFefoDetailsAsync(productId);

            return details.Select(d => new ReceiptDetailDto
            {
                Id = d.Id,
                ProductId = d.ProductId,
                BatchNumber = d.BatchNumber,
                ExpirationDate = d.ExpirationDate,
                QuantityImported = d.QuantityImported,
                ImportPrice = d.ImportPrice
            }).ToList();
        }
    }
}