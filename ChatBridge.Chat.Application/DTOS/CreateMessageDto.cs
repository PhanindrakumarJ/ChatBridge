using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBridge.Chat.Application.DTOS
{
    public class CreateMessageDto
    {
        public Guid SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
