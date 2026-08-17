using BlueCrown.Api.DTOs.ChatMessages;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IChatMessageService
    {
        Task<List<ChatMessageDto>> GetBySessionIdAsync(Guid sessionId, Guid userId);

        Task<ChatMessageDto?> GetByIdAsync(Guid id, Guid userId);

        Task<ChatMessageDto> CreateAsync(Guid userId, CreateChatMessageDto dto);

        Task<bool> MarkAsReadAsync(Guid id, Guid userId);
    }
}