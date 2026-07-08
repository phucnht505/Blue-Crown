using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class ChatMessage
{
    public Guid Id { get; set; }

    public Guid SessionId { get; set; }

    public Guid SenderId { get; set; }

    public string Message { get; set; } = null!;

    public bool? IsRead { get; set; }

    public DateTime? SentAt { get; set; }

    public virtual User Sender { get; set; } = null!;

    public virtual ChatSession Session { get; set; } = null!;
}
