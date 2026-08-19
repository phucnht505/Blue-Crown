using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetByUserIdAsync(Guid userId);
        Task<List<Notification>> GetUnreadByUserIdAsync(Guid userId);
        Task<Notification?> GetByIdAsync(Guid id);
        Task AddAsync(Notification notification);
        Task UpdateAsync(Notification notification);
        Task SaveChangesAsync();
    }
}