using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBridge.Chat.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string? OrganizationName { get; set; }

        public string? OrganizationDescription { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
