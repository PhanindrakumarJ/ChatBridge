using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBridge.Chat.Domain.Entities
{
    public class Message
    {
        public Guid Id { get; set; }

        public Guid ConversationId { get; set; }

        public Guid SenderId { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime SentAt { get; set; }

        // Navigation properties
        public Conversation Conversation { get; set; } = null!;

        public User Sender { get; set; } = null!;
    }
}
