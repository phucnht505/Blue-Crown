using BlueCrown.Api.DTOs.InventoryReceipts;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Services.Implementations
{
    public class InventoryReceiptService : IInventoryReceiptService
    {
        private readonly IInventoryReceiptRepository _receiptRepository;
        private readonly BlueCrownContext _context;

        public InventoryReceiptService(IInventoryReceiptRepository receiptRepository, BlueCrownContext context)
        {
            _receiptRepository = receiptRepository;
            _context = context;
        }

        public async Task<IEnumerable<InventoryReceiptDto>> GetAllAsync()
        {
            var receipts = await _receiptRepository.GetAllAsync();

            return receipts.Select(r => new InventoryReceiptDto
            {
                Id = r.Id,
                SupplierId = r.SupplierId,
                CreatedBy = r.CreatedBy,
                ApprovedBy = r.ApprovedBy,
                TotalCost = r.TotalCost,
                ReceiptDate = r.ReceiptDate,
                Status = r.Status,

                Details = r.ReceiptDetails.Select(d => new ReceiptDetailDto
                {
                    Id = d.Id,
                    ProductId = d.ProductId,
                    BatchNumber = d.BatchNumber,
                    ExpirationDate = d.ExpirationDate,
                    QuantityImported = d.QuantityImported,
                    ImportPrice = d.ImportPrice
                }).ToList()
            });
        }

        public async Task<InventoryReceiptDto?> GetByIdAsync(Guid id)
        {
            var r = await _receiptRepository.GetByIdAsync(id);

            if (r == null)
                return null;

            return new InventoryReceiptDto
            {
                Id = r.Id,
                SupplierId = r.SupplierId,
                CreatedBy = r.CreatedBy,
                ApprovedBy = r.ApprovedBy,
                TotalCost = r.TotalCost,
                ReceiptDate = r.ReceiptDate,
                Status = r.Status,

                Details = r.ReceiptDetails.Select(d => new ReceiptDetailDto
                {
                    Id = d.Id,
                    ProductId = d.ProductId,
                    BatchNumber = d.BatchNumber,
                    ExpirationDate = d.ExpirationDate,
                    QuantityImported = d.QuantityImported,
                    ImportPrice = d.ImportPrice
                }).ToList()
            };
        }

        public async Task CreateAsync(CreateInventoryReceiptDto dto, Guid userId)
        {
            if (dto.Details == null || dto.Details.Count == 0)
                throw new Exception("Phiếu nhập phải có ít nhất một thuốc.");

            decimal totalCost = 0;

            var receipt = new InventoryReceipt
            {
                Id = Guid.NewGuid(),
                SupplierId = dto.SupplierId,
                CreatedBy = userId,
                ApprovedBy = null,
                ReceiptDate = DateTime.Now,
                Status = "pending_approval",
                TotalCost = 0
            };

            foreach (var detailDto in dto.Details)
            {
                if (detailDto.ExpirationDate <= DateOnly.FromDateTime(DateTime.Now))
                    throw new Exception($"Thuốc có lô {detailDto.BatchNumber} đã hết hạn hoặc có hạn dùng không hợp lệ.");

                var detail = new ReceiptDetail
                {
                    Id = Guid.NewGuid(),
                    ReceiptId = receipt.Id,
                    ProductId = detailDto.ProductId,
                    BatchNumber = detailDto.BatchNumber,
                    ExpirationDate = detailDto.ExpirationDate,
                    QuantityImported = detailDto.QuantityImported,
                    ImportPrice = detailDto.ImportPrice
                };

                receipt.ReceiptDetails.Add(detail);

                totalCost += detailDto.QuantityImported * detailDto.ImportPrice;
            }

            receipt.TotalCost = totalCost;

            await _receiptRepository.AddAsync(receipt);
            await _receiptRepository.SaveChangesAsync();
        }

        public async Task<bool> ApproveAsync(Guid receiptId, Guid adminId)
        {
            var receipt = await _receiptRepository.GetByIdAsync(receiptId);
            if (receipt == null)
                return false;
            if (receipt.Status != "pending_approval")
                throw new Exception("Chỉ có thể duyệt phiếu nhập đang ở trạng thái pending_approval.");
            foreach (var detail in receipt.ReceiptDetails)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == detail.ProductId);
                if (product == null)
                    throw new Exception($"Không tìm thấy thuốc có Id: {detail.ProductId}");

                product.StockQuantity = (product.StockQuantity ?? 0) + detail.QuantityImported;
            }

            receipt.ApprovedBy = adminId;
            receipt.Status = "approved";
            await _receiptRepository.SaveChangesAsync();
            return true;
        }
    }
}