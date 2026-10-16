using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBridge.Chat.Domain.Entities
{
    public class Conversation
    {
        public Guid Id { get; set; }

        public Guid VendorId { get; set; }

        public Guid CustomerId { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User Vendor { get; set; } = null!;

        public User Customer { get; set; } = null!;

        public ICollection<Message> Messages { get; set; }
            = new List<Message>();
    }
}
