using System;
using System.Collections.Generic;

namespace ChatBridge.Chat.Infrastructure.Data.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string? OrganizationName { get; set; }

    public string? OrganizationDescription { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Conversation> ConversationCustomers { get; set; } = new List<Conversation>();

    public virtual ICollection<Conversation> ConversationVendors { get; set; } = new List<Conversation>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
