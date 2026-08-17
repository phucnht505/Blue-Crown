using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IChatMessageRepository
    {
        Task<List<ChatMessage>> GetBySessionIdAsync(Guid sessionId);

        Task<ChatMessage?> GetByIdAsync(Guid id);

        Task AddAsync(ChatMessage message);

        Task UpdateAsync(ChatMessage message);

        Task SaveChangesAsync();
    }
}