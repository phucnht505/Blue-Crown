using BlueCrown.Api.DTOs.Checkout;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICheckoutRepository _checkoutRepository;
        private readonly BlueCrownContext _context;

        public CheckoutService(ICheckoutRepository checkoutRepository, BlueCrownContext context)
        {
            _checkoutRepository = checkoutRepository;
            _context = context;
        }

        public async Task<CheckoutResponseDto> CreateAsync(CreateCheckoutDto dto, Guid? userId)
        {
            // BR-CHECKOUT-001: Đơn hàng phải có ít nhất một sản phẩm.
            if (dto.Items == null || dto.Items.Count == 0)
                throw new Exception("Đơn hàng phải có ít nhất một sản phẩm.");

            // BR-CHECKOUT-002: Địa chỉ giao hàng phải hợp lệ.
            var shippingAddress = dto.ShippingAddress?.Trim();

            if (string.IsNullOrWhiteSpace(shippingAddress))
                throw new Exception("Địa chỉ giao hàng không được để trống.");

            if (shippingAddress.Length < 10 || shippingAddress.Length > 500)
                throw new Exception("Địa chỉ giao hàng phải từ 10 đến 500 ký tự.");

            // BR-CHECKOUT-003: Chỉ chấp nhận các phương thức thanh toán được hệ thống hỗ trợ.
            var paymentMethod = dto.PaymentMethod?.Trim().ToLowerInvariant();
            var allowedPaymentMethods = new[] { "cod", "momo", "vnpay" };

            if (string.IsNullOrWhiteSpace(paymentMethod) || !allowedPaymentMethods.Contains(paymentMethod))
                throw new Exception("Phương thức thanh toán không hợp lệ.");

            // BR-CHECKOUT-004: Guest bắt buộc phải có số điện thoại hợp lệ.
            string? guestPhone = dto.GuestPhone?.Trim();

            if (!userId.HasValue)
            {
                if (string.IsNullOrWhiteSpace(guestPhone))
                    throw new Exception("Khách chưa đăng nhập phải cung cấp số điện thoại.");

                guestPhone = NormalizePhone(guestPhone);

                if (!IsValidVietnamPhone(guestPhone))
                    throw new Exception("Số điện thoại không hợp lệ.");
            }

            // BR-CHECKOUT-005: Không cho cùng một Product xuất hiện nhiều lần trong request.
            var duplicateProduct = dto.Items
                .GroupBy(item => item.ProductId)
                .FirstOrDefault(group => group.Count() > 1);

            if (duplicateProduct != null)
                throw new Exception("Một sản phẩm không được xuất hiện nhiều lần trong đơn hàng.");

            var order = new EcommerceOrder
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                GuestPhone = userId.HasValue ? null : guestPhone,
                ShippingAddress = shippingAddress,
                PaymentMethod = paymentMethod,
                PaymentStatus = "pending",
                OrderStatus = "pending",
                PrescriptionId = dto.PrescriptionId,
                CreatedAt = DateTime.Now,
                TotalAmount = 0
            };

            decimal totalAmount = 0;

            foreach (var itemDto in dto.Items)
            {
                // BR-CHECKOUT-006: Số lượng phải lớn hơn 0.
                if (itemDto.Quantity <= 0)
                    throw new Exception("Số lượng sản phẩm phải lớn hơn 0.");

                var product = await _context.Products.FindAsync(itemDto.ProductId);

                // BR-CHECKOUT-007: Product phải tồn tại.
                if (product == null)
                    throw new Exception($"Không tìm thấy sản phẩm {itemDto.ProductId}.");

                // BR-CHECKOUT-008: Product phải còn hàng.
                var stockQuantity = product.StockQuantity ?? 0;

                if (stockQuantity <= 0)
                    throw new Exception($"Sản phẩm \"{product.Name}\" hiện đã hết hàng.");

                // BR-CHECKOUT-009: Không được đặt vượt tồn kho.
                if (itemDto.Quantity > stockQuantity)
                    throw new Exception(
                        $"Sản phẩm \"{product.Name}\" chỉ còn {stockQuantity} sản phẩm trong kho.");

                // BR-CHECKOUT-010: Giá phải được lấy từ database, không lấy từ Frontend.
                var unitPrice = product.Price;

                if (unitPrice < 0)
                    throw new Exception($"Giá sản phẩm \"{product.Name}\" không hợp lệ.");

                var itemTotal = unitPrice * itemDto.Quantity;

                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = itemDto.Quantity,
                    UnitPrice = unitPrice
                };

                order.OrderItems.Add(orderItem);
                totalAmount += itemTotal;
            }

            // BR-CHECKOUT-011: Tổng tiền phải được Backend tự tính.
            order.TotalAmount = totalAmount;

            await _checkoutRepository.AddAsync(order);
            await _checkoutRepository.SaveChangesAsync();

            return MapToDto(order);
        }

        public async Task<CheckoutResponseDto?> GetByIdAsync(Guid id)
        {
            var order = await _checkoutRepository.GetByIdAsync(id);

            if (order == null)
                return null;

            return MapToDto(order);
        }

        private static string NormalizePhone(string phone)
        {
            var normalized = phone
                .Replace(" ", "")
                .Replace(".", "")
                .Replace("-", "");

            if (normalized.StartsWith("+84"))
                normalized = "0" + normalized[3..];

            return normalized;
        }

        private static bool IsValidVietnamPhone(string phone)
        {
            if (phone.Length != 10 || phone[0] != '0')
                return false;

            if (phone[1] != '3' &&
                phone[1] != '5' &&
                phone[1] != '7' &&
                phone[1] != '8' &&
                phone[1] != '9')
                return false;

            return phone.All(char.IsDigit);
        }

        private CheckoutResponseDto MapToDto(EcommerceOrder order)
        {
            return new CheckoutResponseDto
            {
                Id = order.Id,
                UserId = order.UserId,
                GuestPhone = order.GuestPhone,
                ShippingAddress = order.ShippingAddress,
                TotalAmount = order.TotalAmount,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                OrderStatus = order.OrderStatus,
                PrescriptionId = order.PrescriptionId,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems.Select(item => new CheckoutItemResponseDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.UnitPrice * item.Quantity
                }).ToList()
            };
        }
    }
}