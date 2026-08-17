using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly BlueCrownContext _context;

        public ChatMessageRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task<List<ChatMessage>> GetBySessionIdAsync(Guid sessionId)
        {
            return await _context.ChatMessages
                .AsNoTracking()
                .Where(x => x.SessionId == sessionId)
                .OrderBy(x => x.SentAt)
                .ToListAsync();
        }

        public async Task<ChatMessage?> GetByIdAsync(Guid id)
        {
            return await _context.ChatMessages.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(ChatMessage message)
        {
            await _context.ChatMessages.AddAsync(message);
        }

        public async Task UpdateAsync(ChatMessage message)
        {
            _context.ChatMessages.Update(message);

            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}