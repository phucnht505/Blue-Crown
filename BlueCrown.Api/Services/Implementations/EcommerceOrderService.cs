using BlueCrown.Api.DTOs.EcommerceOrders;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class EcommerceOrderService : IEcommerceOrderService
    {
        private readonly IEcommerceOrderRepository _repository;

        public EcommerceOrderService(
            IEcommerceOrderRepository repository)
        {
            _repository = repository;
        }
        public async Task<List<EcommerceOrderDto>> GetAllAsync()
        {
            var orders = await _repository.GetAllAsync();

            return orders
                .Select(MapToDto)
                .ToList();
        }

        public async Task<EcommerceOrderDto?> GetByIdAsync(Guid id)
        {
            var order = await _repository.GetByIdAsync(id);

            if (order == null)
            {
                return null;
            }

            return MapToDto(order);
        }

        public async Task<EcommerceOrderDto> CreateAsync(
    CreateEcommerceOrderDto dto)
        {
            if (dto.UserId.HasValue)
            {
                var userExists = await _repository.UserExistsAsync(dto.UserId.Value);

                if (!userExists)
                {
                    throw new Exception(
                        $"User {dto.UserId.Value} không tồn tại trong database."
                    );
                }
            }
            var order = new EcommerceOrder
            {
                Id = Guid.NewGuid(),

                UserId = dto.UserId,

                GuestPhone = dto.GuestPhone,

                ShippingAddress = dto.ShippingAddress,

                TotalAmount = dto.TotalAmount,

                PaymentMethod = dto.PaymentMethod,

                PaymentStatus = "Pending",

                OrderStatus = "Pending",

                PrescriptionId = dto.PrescriptionId,

                CreatedAt = DateTime.UtcNow
            };

            order.OrderItems = dto.Items
                .Select(item => new OrderItem
                {
                    Id = Guid.NewGuid(),

                    OrderId = order.Id,

                    ProductId = item.ProductId,

                    Quantity = item.Quantity,

                    UnitPrice = item.UnitPrice
                })
                .ToList();

            await _repository.AddAsync(order);

            await _repository.SaveChangesAsync();

            return MapToDto(order);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var order = await _repository.GetByIdAsync(id);

            if (order == null)
            {
                return false;
            }

            await _repository.DeleteAsync(order);

            await _repository.SaveChangesAsync();

            return true;
        }
        private static EcommerceOrderDto MapToDto(
            EcommerceOrder order)
        {
            return new EcommerceOrderDto
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

                Items = order.OrderItems
                    .Select(item => new OrderItemDto
                    {
                        Id = item.Id,

                        ProductId = item.ProductId,

                        Quantity = item.Quantity,

                        UnitPrice = item.UnitPrice
                    })
                    .ToList()
            };
        }
    }
}