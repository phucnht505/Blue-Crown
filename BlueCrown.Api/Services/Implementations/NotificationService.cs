using BlueCrown.Api.DTOs.Notifications;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;

        public NotificationService(INotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<NotificationDto>> GetMyNotificationsAsync(Guid userId)
        {
            var notifications = await _repository.GetByUserIdAsync(userId);

            return notifications.Select(MapToDto).ToList();
        }

        public async Task<List<NotificationDto>> GetMyUnreadNotificationsAsync(Guid userId)
        {
            var notifications = await _repository.GetUnreadByUserIdAsync(userId);

            return notifications.Select(MapToDto).ToList();
        }

        public async Task<NotificationDto?> GetByIdAsync(Guid id, Guid userId)
        {
            var notification = await _repository.GetByIdAsync(id);

            if (notification == null)
                return null;

            if (notification.UserId != userId)
                throw new UnauthorizedAccessException(
                    "You do not have access to this notification.");

            return MapToDto(notification);
        }

        public async Task<NotificationDto> CreateAsync(CreateNotificationDto dto)
        {
            if (dto.UserId == Guid.Empty)
                throw new ArgumentException("UserId không hợp lệ.");

            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title không được để trống.");

            if (string.IsNullOrWhiteSpace(dto.Message))
                throw new ArgumentException("Message không được để trống.");

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                Type = string.IsNullOrWhiteSpace(dto.Type) ? null : dto.Type.Trim(),
                Title = dto.Title.Trim(),
                Message = dto.Message.Trim(),
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(notification);
            await _repository.SaveChangesAsync();

            return MapToDto(notification);
        }

        public async Task<bool> MarkAsReadAsync(Guid id, Guid userId)
        {
            var notification = await _repository.GetByIdAsync(id);

            if (notification == null)
                return false;

            if (notification.UserId != userId)
                throw new UnauthorizedAccessException(
                    "You do not have access to this notification.");

            notification.IsRead = true;

            await _repository.UpdateAsync(notification);
            await _repository.SaveChangesAsync();

            return true;
        }

        private static NotificationDto MapToDto(Notification x)
        {
            return new NotificationDto
            {
                Id = x.Id,
                UserId = x.UserId,
                Type = x.Type,
                Title = x.Title,
                Message = x.Message,
                IsRead = x.IsRead ?? false,
                CreatedAt = x.CreatedAt
            };
        }
    }
}