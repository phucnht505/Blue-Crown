using BlueCrown.Api.DTOs.Notifications;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetMyNotificationsAsync(Guid userId);
        Task<List<NotificationDto>> GetMyUnreadNotificationsAsync(Guid userId);
        Task<NotificationDto?> GetByIdAsync(Guid id, Guid userId);
        Task<NotificationDto> CreateAsync(CreateNotificationDto dto);
        Task<bool> MarkAsReadAsync(Guid id, Guid userId);
    }
}