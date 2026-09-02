using BlueCrown.Api.DTOs.InventoryReceipts;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class InventoryReceiptService : IInventoryReceiptService
    {
        private readonly IInventoryReceiptRepository _receiptRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IProductRepository _productRepository;

        public InventoryReceiptService(IInventoryReceiptRepository receiptRepository, ISupplierRepository supplierRepository, IProductRepository productRepository)
        {
            _receiptRepository = receiptRepository;
            _supplierRepository = supplierRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<InventoryReceiptDto>> GetAllAsync()
        {
            var receipts = await _receiptRepository.GetAllAsync();
            return receipts.Select(MapToDto);
        }

        public async Task<InventoryReceiptDto?> GetByIdAsync(Guid id)
        {
            var receipt = await _receiptRepository.GetByIdAsync(id);
            return receipt == null ? null : MapToDto(receipt);
        }

        public async Task CreateAsync(CreateInventoryReceiptDto dto, Guid userId)
        {
            if (!dto.SupplierId.HasValue || dto.SupplierId.Value == Guid.Empty)
                throw new ArgumentException("Vui lòng chọn nhà cung cấp.");

            var supplier = await _supplierRepository.GetByIdAsync(dto.SupplierId.Value);

            // BR-INV-001: Supplier phải tồn tại.
            if (supplier == null)
                throw new ArgumentException("Nhà cung cấp không tồn tại.");

            // BR-INV-002: Chỉ nhập hàng từ Supplier đạt GDP.
            if (supplier.GdpCertified != true)
                throw new InvalidOperationException("Nhà cung cấp chưa đạt chứng nhận GDP.");

            if (dto.Details == null || dto.Details.Count == 0)
                throw new ArgumentException("Phiếu nhập phải có ít nhất một sản phẩm.");

            if (dto.Details.Any(d => d.ProductId == Guid.Empty))
                throw new ArgumentException("Có Product không hợp lệ trong phiếu nhập.");

            // BR-INV-003: Không được trùng Product + BatchNumber trong cùng phiếu.
            var duplicatedBatch = dto.Details.GroupBy(d => new { d.ProductId, BatchNumber = d.BatchNumber.Trim().ToLower() }).Any(g => g.Count() > 1);
            if (duplicatedBatch)
                throw new InvalidOperationException("Không được nhập trùng cùng Product và số lô trong một phiếu.");

            var productIds = dto.Details.Select(d => d.ProductId).Distinct().ToList();
            var products = await _productRepository.GetByIdsForUpdateAsync(productIds);

            // BR-INV-004: Tất cả Product phải tồn tại.
            if (products.Count != productIds.Count)
                throw new ArgumentException("Có Product trong phiếu nhập không tồn tại.");

            var today = DateOnly.FromDateTime(DateTime.Now);

            foreach (var detail in dto.Details)
            {
                if (string.IsNullOrWhiteSpace(detail.BatchNumber))
                    throw new ArgumentException("Số lô không được để trống.");

                // BR-INV-005: Hạn dùng phải sau ngày hiện tại.
                if (detail.ExpirationDate <= today)
                    throw new InvalidOperationException($"Lô '{detail.BatchNumber.Trim()}' đã hết hạn hoặc có hạn dùng không hợp lệ.");

                if (detail.QuantityImported <= 0)
                    throw new ArgumentException("Số lượng nhập phải lớn hơn 0.");

                if (detail.ImportPrice <= 0)
                    throw new ArgumentException("Giá nhập phải lớn hơn 0.");
            }

            var receipt = new InventoryReceipt
            {
                Id = Guid.NewGuid(),
                SupplierId = supplier.Id,
                CreatedBy = userId,
                ApprovedBy = null,
                TotalCost = dto.Details.Sum(d => d.QuantityImported * d.ImportPrice),
                ReceiptDate = DateTime.UtcNow,
                Status = "pending_approval"
            };

            receipt.ReceiptDetails = dto.Details.Select(detail => new ReceiptDetail
            {
                Id = Guid.NewGuid(),
                ReceiptId = receipt.Id,
                ProductId = detail.ProductId,
                BatchNumber = detail.BatchNumber.Trim(),
                ExpirationDate = detail.ExpirationDate,
                QuantityImported = detail.QuantityImported,
                ImportPrice = detail.ImportPrice
            }).ToList();

            await _receiptRepository.AddAsync(receipt);
            await _receiptRepository.SaveChangesAsync();
        }

        public async Task<bool> ApproveAsync(Guid receiptId, Guid adminId)
        {
            var receipt = await _receiptRepository.GetByIdAsync(receiptId);

            if (receipt == null)
                return false;

            // BR-INV-006: Chỉ phiếu pending_approval mới được duyệt.
            if (!string.Equals(receipt.Status, "pending_approval", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Chỉ có thể duyệt phiếu nhập đang chờ duyệt.");

            if (receipt.ReceiptDetails.Count == 0)
                throw new InvalidOperationException("Phiếu nhập không có chi tiết sản phẩm.");

            var today = DateOnly.FromDateTime(DateTime.Now);

            // BR-INV-007: Không duyệt lô đã hết hạn tại thời điểm duyệt.
            if (receipt.ReceiptDetails.Any(d => d.ExpirationDate <= today))
                throw new InvalidOperationException("Phiếu nhập có lô đã hết hạn hoặc không còn hợp lệ.");

            var productIds = receipt.ReceiptDetails.Where(d => d.ProductId.HasValue).Select(d => d.ProductId!.Value).Distinct().ToList();

            if (productIds.Count == 0)
                throw new InvalidOperationException("Phiếu nhập không có Product hợp lệ.");

            var products = await _productRepository.GetByIdsForUpdateAsync(productIds);

            if (products.Count != productIds.Count)
                throw new InvalidOperationException("Có Product trong phiếu nhập không còn tồn tại.");

            foreach (var detail in receipt.ReceiptDetails)
            {
                if (!detail.ProductId.HasValue)
                    throw new InvalidOperationException("Chi tiết phiếu nhập có Product không hợp lệ.");

                var product = products.First(p => p.Id == detail.ProductId.Value);
                product.StockQuantity = (product.StockQuantity ?? 0) + detail.QuantityImported;
            }

            receipt.ApprovedBy = adminId;
            receipt.Status = "approved";

            // BR-INV-008: Tăng tồn kho và duyệt phiếu trong cùng một SaveChanges.
            await _receiptRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RejectAsync(Guid receiptId, Guid adminId)
        {
            var receipt = await _receiptRepository.GetByIdAsync(receiptId);

            if (receipt == null)
                return false;

            // BR-INV-009: Chỉ phiếu pending_approval mới được từ chối.
            if (!string.Equals(receipt.Status, "pending_approval", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Chỉ có thể từ chối phiếu nhập đang chờ duyệt.");

            receipt.ApprovedBy = adminId;
            receipt.Status = "rejected";

            await _receiptRepository.SaveChangesAsync();

            return true;
        }

        private static InventoryReceiptDto MapToDto(InventoryReceipt receipt)
        {
            return new InventoryReceiptDto
            {
                Id = receipt.Id,
                SupplierId = receipt.SupplierId,
                SupplierName = receipt.Supplier?.SupplierName,
                CreatedBy = receipt.CreatedBy,
                CreatedByName = receipt.CreatedByNavigation?.FullName,
                ApprovedBy = receipt.ApprovedBy,
                ApprovedByName = receipt.ApprovedByNavigation?.FullName,
                TotalCost = receipt.TotalCost,
                ReceiptDate = receipt.ReceiptDate,
                Status = receipt.Status,
                Details = receipt.ReceiptDetails.Select(detail => new ReceiptDetailDto
                {
                    Id = detail.Id,
                    ProductId = detail.ProductId,
                    ProductName = detail.Product?.Name,
                    StockQuantity = detail.Product?.StockQuantity,
                    BatchNumber = detail.BatchNumber,
                    ExpirationDate = detail.ExpirationDate,
                    QuantityImported = detail.QuantityImported,
                    ImportPrice = detail.ImportPrice
                }).ToList()
            };
        }
    }
}