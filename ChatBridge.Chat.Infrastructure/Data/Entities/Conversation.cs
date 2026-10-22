using System;
using System.Collections.Generic;

namespace ChatBridge.Chat.Infrastructure.Data.Entities;

public partial class Conversation
{
    public Guid Id { get; set; }

    public Guid VendorId { get; set; }

    public Guid CustomerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual User Vendor { get; set; } = null!;
}
