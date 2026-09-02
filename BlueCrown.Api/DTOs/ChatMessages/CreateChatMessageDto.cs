using System;
using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.ChatMessages
{
    public class CreateChatMessageDto
    {
        public Guid SessionId { get; set; }

        [Required(ErrorMessage = "Nội dung tin nhắn không được để trống.")]
        [StringLength(2000, ErrorMessage = "Tin nhắn không được vượt quá 2000 ký tự.")]
        public string Message { get; set; } = null!;
    }
}