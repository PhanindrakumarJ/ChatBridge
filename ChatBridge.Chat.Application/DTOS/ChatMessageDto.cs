using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBridge.Chat.Application.DTOS
{
    public class ChatMessageDto
    {
        public Guid Id { get; set;  }
        public Guid SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }
}
