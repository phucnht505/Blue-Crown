using BlueCrown.Api.DTOs.EcommerceOrders;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;
using System.Text.RegularExpressions;

namespace BlueCrown.Api.Services.Implementations
{
    public class EcommerceOrderService : IEcommerceOrderService
    {
        private readonly IEcommerceOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPrescriptionRepository _prescriptionRepository;

        public EcommerceOrderService(IEcommerceOrderRepository orderRepository, IProductRepository productRepository, IPrescriptionRepository prescriptionRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _prescriptionRepository = prescriptionRepository;
        }

        public async Task<List<EcommerceOrderDto>> GetManagementOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(MapToDto).ToList();
        }

        public async Task<EcommerceOrderDto?> GetManagementOrderByIdAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            return order == null ? null : MapToDto(order);
        }

        public async Task<List<EcommerceOrderDto>> GetMyOrdersAsync(Guid userId)
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);
            return orders.Select(MapToDto).ToList();
        }

        public async Task<EcommerceOrderDto?> GetMyOrderByIdAsync(Guid id, Guid userId)
        {
            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null || order.UserId != userId)
                return null;

            return MapToDto(order);
        }

        public async Task<List<EcommerceOrderDto>> LookupGuestOrdersAsync(GuestOrderLookupDto dto)
        {
            var phone = NormalizePhone(dto.GuestPhone);

            if (!Regex.IsMatch(phone, @"^0[35789]\d{8}$"))
                throw new ArgumentException("Số điện thoại không hợp lệ. Ví dụ: 0901234567.");

            // BR-ORD-LOOKUP-001: Nếu có mã đơn thì mã đơn và SĐT phải cùng khớp.
            if (dto.OrderId.HasValue)
            {
                if (dto.OrderId.Value == Guid.Empty)
                    throw new ArgumentException("Mã đơn hàng không hợp lệ.");

                var order = await _orderRepository.GetByIdAsync(dto.OrderId.Value);

                if (order == null || order.UserId.HasValue)
                    return new List<EcommerceOrderDto>();

                if (!string.Equals(NormalizePhone(order.GuestPhone), phone, StringComparison.Ordinal))
                    return new List<EcommerceOrderDto>();

                return new List<EcommerceOrderDto> { MapToDto(order) };
            }

            // BR-ORD-LOOKUP-002: Không có mã đơn thì tìm các đơn Guest theo SĐT.
            var orders = await _orderRepository.GetGuestOrdersByPhoneAsync(phone);
            return orders.Select(MapToDto).ToList();
        }

        public async Task<EcommerceOrderDto> CreateAsync(Guid? userId, CreateEcommerceOrderDto dto)
        {
            ValidateCreateRequest(dto);

            await using var transaction = await _orderRepository.BeginSerializableTransactionAsync();

            try
            {
                var productIds = dto.Items.Select(x => x.ProductId).Distinct().ToList();

                // BR-ORD-001: Không cho Product trùng trong cùng một Order.
                if (productIds.Count != dto.Items.Count)
                    throw new InvalidOperationException("Không được có Product trùng trong cùng một đơn hàng.");

                var products = await _productRepository.GetByIdsForUpdateAsync(productIds);

                // BR-ORD-002: Tất cả Product phải tồn tại.
                if (products.Count != productIds.Count)
                    throw new ArgumentException("Có Product trong giỏ hàng không còn tồn tại.");

                var productMap = products.ToDictionary(x => x.Id);

                foreach (var item in dto.Items)
                {
                    if (item.ProductId == Guid.Empty)
                        throw new ArgumentException("Product không hợp lệ.");

                    if (item.Quantity < 1 || item.Quantity > 99)
                        throw new ArgumentException("Số lượng mỗi Product phải từ 1 đến 99.");

                    var product = productMap[item.ProductId];
                    var stock = product.StockQuantity ?? 0;

                    // BR-ORD-003: Không cho mua Product hết hàng hoặc vượt tồn kho.
                    if (stock <= 0)
                        throw new InvalidOperationException($"Product '{product.Name}' đã hết hàng.");

                    if (item.Quantity > stock)
                        throw new InvalidOperationException($"Product '{product.Name}' chỉ còn {stock} sản phẩm.");
                }

                await ValidatePrescriptionAsync(userId, dto.PrescriptionId, dto.Items, productMap);

                decimal totalAmount = 0;

                foreach (var item in dto.Items)
                {
                    var product = productMap[item.ProductId];
                    totalAmount += product.Price * item.Quantity;

                    // BR-ORD-004: Tạo Order giữ hàng ngay để tránh bán vượt tồn kho.
                    product.StockQuantity = (product.StockQuantity ?? 0) - item.Quantity;
                }

                var order = new EcommerceOrder
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    GuestPhone = NormalizePhone(dto.GuestPhone),
                    ShippingAddress = dto.ShippingAddress.Trim(),
                    TotalAmount = totalAmount,
                    PaymentMethod = "cod",
                    PaymentStatus = "pending",
                    OrderStatus = "processing",
                    PrescriptionId = dto.PrescriptionId,
                    CreatedAt = DateTime.UtcNow
                };

                order.OrderItems = dto.Items.Select(item =>
                {
                    var product = productMap[item.ProductId];

                    return new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    };
                }).ToList();

                // BR-ORD-005: Giá và tổng tiền luôn lấy từ database.
                await _orderRepository.AddAsync(order);
                await _orderRepository.SaveChangesAsync();
                await transaction.CommitAsync();

                var createdOrder = await _orderRepository.GetByIdAsync(order.Id);

                if (createdOrder == null)
                    throw new InvalidOperationException("Không thể tải lại đơn hàng vừa tạo.");

                return MapToDto(createdOrder);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<EcommerceOrderDto?> UpdateStatusAsync(Guid id, UpdateEcommerceOrderStatusDto dto)
        {
            var targetStatus = dto.Status.Trim().ToLowerInvariant();

            if (targetStatus is not ("confirmed" or "shipped" or "delivered" or "cancelled"))
                throw new ArgumentException("Trạng thái đơn hàng không hợp lệ.");

            await using var transaction = await _orderRepository.BeginSerializableTransactionAsync();

            try
            {
                var order = await _orderRepository.GetByIdForUpdateAsync(id);

                if (order == null)
                    return null;

                var currentStatus = NormalizeOrderStatus(order.OrderStatus);

                // BR-ORD-006: Chỉ cho chuyển trạng thái đúng luồng nghiệp vụ.
                if (!CanTransition(currentStatus, targetStatus))
                    throw new InvalidOperationException($"Không thể chuyển đơn hàng từ '{GetStatusText(currentStatus)}' sang '{GetStatusText(targetStatus)}'.");

                if (targetStatus == "cancelled")
                {
                    RestoreStock(order);
                    order.PaymentStatus = "cancelled";
                }

                order.OrderStatus = targetStatus;

                // BR-ORD-007: COD chỉ được xem đã thanh toán khi giao thành công.
                if (targetStatus == "delivered" && string.Equals(order.PaymentMethod, "cod", StringComparison.OrdinalIgnoreCase))
                    order.PaymentStatus = "paid";

                await _orderRepository.SaveChangesAsync();
                await transaction.CommitAsync();

                var updatedOrder = await _orderRepository.GetByIdAsync(id);
                return updatedOrder == null ? null : MapToDto(updatedOrder);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<EcommerceOrderDto?> CancelMyOrderAsync(Guid id, Guid userId)
        {
            await using var transaction = await _orderRepository.BeginSerializableTransactionAsync();

            try
            {
                var order = await _orderRepository.GetByIdForUpdateAsync(id);

                // BR-ORD-008: User chỉ được hủy Order của chính mình.
                if (order == null || order.UserId != userId)
                    return null;

                var currentStatus = NormalizeOrderStatus(order.OrderStatus);

                // BR-ORD-009: Chỉ được tự hủy khi processing hoặc confirmed.
                if (currentStatus is not ("processing" or "confirmed"))
                    throw new InvalidOperationException("Đơn hàng ở trạng thái hiện tại không thể hủy.");

                RestoreStock(order);

                order.OrderStatus = "cancelled";
                order.PaymentStatus = "cancelled";

                await _orderRepository.SaveChangesAsync();
                await transaction.CommitAsync();

                var updatedOrder = await _orderRepository.GetByIdAsync(id);
                return updatedOrder == null ? null : MapToDto(updatedOrder);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task ValidatePrescriptionAsync(Guid? userId, Guid? prescriptionId, List<CreateOrderItemDto> items, Dictionary<Guid, Product> productMap)
        {
            var prescriptionProducts = items.Select(x => productMap[x.ProductId]).Where(IsPrescriptionProduct).ToList();

            if (prescriptionProducts.Count == 0)
            {
                if (prescriptionId.HasValue)
                    throw new ArgumentException("Đơn hàng không có Product cần kê đơn nên không cần Prescription.");

                return;
            }

            // BR-ORD-RX-001: Product kê đơn yêu cầu tài khoản đăng nhập.
            if (!userId.HasValue)
                throw new InvalidOperationException("Product cần đơn thuốc yêu cầu người mua đăng nhập.");

            if (!prescriptionId.HasValue || prescriptionId.Value == Guid.Empty)
                throw new ArgumentException("Đơn hàng có Product cần kê đơn. Vui lòng chọn Prescription hợp lệ.");

            var prescription = await _prescriptionRepository.GetByIdAsync(prescriptionId.Value);

            if (prescription == null)
                throw new ArgumentException("Prescription không tồn tại.");

            // BR-ORD-RX-002: Không được sử dụng Prescription của người khác.
            if (prescription.Patient?.UserId != userId.Value)
                throw new InvalidOperationException("Prescription không thuộc tài khoản đang đặt hàng.");

            // BR-ORD-RX-003: Prescription phải được Pharmacist duyệt.
            if (!string.Equals(prescription.Status, "approved", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Prescription phải ở trạng thái đã duyệt trước khi dùng để đặt hàng.");

            // BR-ORD-RX-004: Không dùng lại Prescription cho Order khác đang hoạt động.
            if (await _orderRepository.HasActiveOrderByPrescriptionIdAsync(prescription.Id))
                throw new InvalidOperationException("Prescription này đã được sử dụng cho một đơn hàng khác.");

            var medicationIds = prescription.PrescriptionItems.Select(x => x.MedicationId).ToHashSet();

            // BR-ORD-RX-005: Product kê đơn phải đúng Medication bác sĩ kê.
            foreach (var product in prescriptionProducts)
            {
                if (!product.MedicationId.HasValue)
                    throw new InvalidOperationException($"Product '{product.Name}' chưa được liên kết Medication.");

                if (!medicationIds.Contains(product.MedicationId.Value))
                    throw new InvalidOperationException($"Product '{product.Name}' không phù hợp với Prescription được chọn.");
            }
        }

        private static void ValidateCreateRequest(CreateEcommerceOrderDto dto)
        {
            var phone = NormalizePhone(dto.GuestPhone);

            if (!Regex.IsMatch(phone, @"^0[35789]\d{8}$"))
                throw new ArgumentException("Số điện thoại không hợp lệ. Ví dụ: 0901234567.");

            var address = dto.ShippingAddress?.Trim() ?? string.Empty;

            if (address.Length < 10 || address.Length > 500)
                throw new ArgumentException("Địa chỉ giao hàng phải từ 10 đến 500 ký tự.");

            if (!address.Any(char.IsLetter))
                throw new ArgumentException("Địa chỉ giao hàng phải chứa thông tin chữ hợp lệ.");

            if (!string.Equals(dto.PaymentMethod?.Trim(), "cod", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Hiện tại hệ thống chỉ hỗ trợ thanh toán khi nhận hàng.");

            if (dto.Items == null || dto.Items.Count == 0)
                throw new ArgumentException("Đơn hàng phải có ít nhất một sản phẩm.");

            if (dto.Items.Count > 50)
                throw new ArgumentException("Một đơn hàng tối đa 50 loại sản phẩm.");
        }

        private static void RestoreStock(EcommerceOrder order)
        {
            foreach (var item in order.OrderItems)
            {
                if (item.Product == null)
                    throw new InvalidOperationException("Không thể hoàn tồn kho vì Product không tồn tại.");

                item.Product.StockQuantity = (item.Product.StockQuantity ?? 0) + item.Quantity;
            }
        }

        private static bool CanTransition(string current, string target)
        {
            return current switch
            {
                "processing" => target is "confirmed" or "cancelled",
                "confirmed" => target is "shipped" or "cancelled",
                "shipped" => target == "delivered",
                _ => false
            };
        }

        private static bool IsPrescriptionProduct(Product product)
        {
            return product.IsPrescriptionRequired == true || product.PrescriptionRequired;
        }

        private static string NormalizePhone(string? phone)
        {
            var value = (phone ?? string.Empty).Trim().Replace(" ", "").Replace(".", "").Replace("-", "");

            if (value.StartsWith("+84"))
                value = $"0{value[3..]}";

            return value;
        }

        private static string NormalizeOrderStatus(string? status)
        {
            var value = status?.Trim().ToLowerInvariant();

            return value switch
            {
                "pending" => "processing",
                "processing" => "processing",
                "confirmed" => "confirmed",
                "shipped" => "shipped",
                "delivered" => "delivered",
                "cancelled" => "cancelled",
                _ => value ?? string.Empty
            };
        }

        private static string NormalizePaymentStatus(string? status)
        {
            return status?.Trim().ToLowerInvariant() ?? "pending";
        }

        private static string GetStatusText(string status)
        {
            return status switch
            {
                "processing" => "đang xử lý",
                "confirmed" => "đã xác nhận",
                "shipped" => "đang giao",
                "delivered" => "đã giao",
                "cancelled" => "đã hủy",
                _ => status
            };
        }

        private static EcommerceOrderDto MapToDto(EcommerceOrder order)
        {
            return new EcommerceOrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = order.User?.FullName,
                GuestPhone = order.GuestPhone,
                ShippingAddress = order.ShippingAddress,
                TotalAmount = order.TotalAmount,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = NormalizePaymentStatus(order.PaymentStatus),
                OrderStatus = NormalizeOrderStatus(order.OrderStatus),
                PrescriptionId = order.PrescriptionId,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name ?? "Không xác định",
                    ProductImageUrl = item.Product?.ImageUrl,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };
        }
    }
}