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
            if (dto.Items == null || dto.Items.Count == 0)
                throw new Exception("Đơn hàng phải có ít nhất một sản phẩm.");

            if (string.IsNullOrWhiteSpace(dto.ShippingAddress))
                throw new Exception("Địa chỉ giao hàng không được để trống.");

            if (string.IsNullOrWhiteSpace(dto.PaymentMethod))
                throw new Exception("Phương thức thanh toán không được để trống.");

            if (!userId.HasValue && string.IsNullOrWhiteSpace(dto.GuestPhone))
                throw new Exception(
                    "Khách chưa đăng nhập phải cung cấp số điện thoại.");

            var order = new EcommerceOrder
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                GuestPhone = dto.GuestPhone,
                ShippingAddress = dto.ShippingAddress,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = "pending",
                OrderStatus = "pending",
                PrescriptionId = dto.PrescriptionId,
                CreatedAt = DateTime.Now,
                TotalAmount = 0
            };

            decimal totalAmount = 0;

            foreach (var itemDto in dto.Items)
            {
                if (itemDto.Quantity <= 0)
                    throw new Exception("Số lượng sản phẩm phải lớn hơn 0.");

                var product = await _context.Products.FindAsync(itemDto.ProductId);

                if (product == null)
                    throw new Exception($"Không tìm thấy sản phẩm {itemDto.ProductId}.");

                var unitPrice = product.Price;
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