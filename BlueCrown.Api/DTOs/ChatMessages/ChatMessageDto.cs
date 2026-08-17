using System;

namespace BlueCrown.Api.DTOs.ChatMessages
{
    public class ChatMessageDto
    {
        public Guid Id { get; set; }

        public Guid SessionId { get; set; }

        public Guid SenderId { get; set; }

        public string Message { get; set; } = null!;

        public bool? IsRead { get; set; }

        public DateTime? SentAt { get; set; }
    }
}