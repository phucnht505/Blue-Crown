using System;

namespace BlueCrown.Api.DTOs.ChatMessages
{
    public class CreateChatMessageDto
    {
        public Guid SessionId { get; set; }

        public string Message { get; set; } = null!;
    }
}