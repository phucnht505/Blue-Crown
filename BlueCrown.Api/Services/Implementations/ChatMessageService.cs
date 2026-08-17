using BlueCrown.Api.DTOs.ChatMessages;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Services.Implementations
{
    public class ChatMessageService : IChatMessageService
    {
        private readonly IChatMessageRepository _repository;
        private readonly BlueCrownContext _context;

        public ChatMessageService(IChatMessageRepository repository, BlueCrownContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<List<ChatMessageDto>> GetBySessionIdAsync(Guid sessionId, Guid userId)
        {
            var session = await _context.ChatSessions
                .FirstOrDefaultAsync(x => x.Id == sessionId);

            if (session == null)
            {
                throw new Exception("Chat session not found.");
            }

            await EnsureUserCanAccessSessionAsync(session, userId);

            var messages = await _repository.GetBySessionIdAsync(sessionId);

            return messages.Select(MapToDto).ToList();
        }

        public async Task<ChatMessageDto?> GetByIdAsync(Guid id, Guid userId)
        {
            var message = await _repository.GetByIdAsync(id);

            if (message == null)
            {
                return null;
            }

            var session = await _context.ChatSessions
                .FirstOrDefaultAsync(x => x.Id == message.SessionId);

            if (session == null)
            {
                throw new Exception("Chat session not found.");
            }

            await EnsureUserCanAccessSessionAsync(session, userId);

            return MapToDto(message);
        }

        public async Task<ChatMessageDto> CreateAsync(Guid userId, CreateChatMessageDto dto)
        {
            // 1. Kiểm tra ChatSession
            var session = await _context.ChatSessions
                .FirstOrDefaultAsync(x => x.Id == dto.SessionId);

            if (session == null)
            {
                throw new Exception("Chat session not found.");
            }

            // 2. Không cho gửi tin nhắn khi session đã đóng
            if (string.Equals(session.Status, "closed", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Cannot send message to a closed chat session.");
            }

            // 3. Kiểm tra User có được tham gia session không
            await EnsureUserCanAccessSessionAsync(session, userId);

            // 4. Kiểm tra nội dung
            if (string.IsNullOrWhiteSpace(dto.Message))
            {
                throw new Exception("Message cannot be empty.");
            }

            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),

                SessionId = dto.SessionId,

                SenderId = userId,

                Message = dto.Message.Trim(),

                IsRead = false,

                SentAt = DateTime.UtcNow
            };

            await _repository.AddAsync(message);

            await _repository.SaveChangesAsync();

            var createdMessage = await _repository.GetByIdAsync(message.Id);

            if (createdMessage == null)
            {
                throw new Exception("Failed to retrieve created chat message.");
            }

            return MapToDto(createdMessage);
        }

        public async Task<bool> MarkAsReadAsync(Guid id, Guid userId)
        {
            var message = await _repository.GetByIdAsync(id);

            if (message == null)
            {
                return false;
            }

            var session = await _context.ChatSessions
                .FirstOrDefaultAsync(x => x.Id == message.SessionId);

            if (session == null)
            {
                throw new Exception("Chat session not found.");
            }

            await EnsureUserCanAccessSessionAsync(session, userId);

            // Người gửi không cần tự đánh dấu tin nhắn của mình là đã đọc.
            if (message.SenderId == userId)
            {
                throw new Exception("Sender cannot mark their own message as read.");
            }

            message.IsRead = true;

            await _repository.UpdateAsync(message);

            await _repository.SaveChangesAsync();

            return true;
        }

        private async Task EnsureUserCanAccessSessionAsync(ChatSession session, Guid userId)
        {
            // User -> PatientProfile
            var patientId = await _context.PatientProfiles
                .Where(p => p.UserId == userId)
                .Select(p => (Guid?)p.Id)
                .FirstOrDefaultAsync();

            if (patientId.HasValue && session.PatientId == patientId.Value)
            {
                return;
            }

            // User -> DoctorProfile
            var doctorId = await _context.DoctorProfiles
                .Where(d => d.UserId == userId)
                .Select(d => (Guid?)d.Id)
                .FirstOrDefaultAsync();

            if (doctorId.HasValue && session.DoctorId == doctorId.Value)
            {
                return;
            }

            throw new UnauthorizedAccessException("You do not have access to this chat session.");
        }

        private static ChatMessageDto MapToDto(ChatMessage message)
        {
            return new ChatMessageDto
            {
                Id = message.Id,

                SessionId = message.SessionId,

                SenderId = message.SenderId,

                Message = message.Message,

                IsRead = message.IsRead,

                SentAt = message.SentAt
            };
        }
    }
}