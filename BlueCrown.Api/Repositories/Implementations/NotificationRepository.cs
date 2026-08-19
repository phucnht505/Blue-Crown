using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly BlueCrownContext _context;

        public NotificationRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Notifications
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetUnreadByUserIdAsync(Guid userId)
        {
            return await _context.Notifications
                .Where(x => x.UserId == userId && x.IsRead != true)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(Guid id)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }

        public Task UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}