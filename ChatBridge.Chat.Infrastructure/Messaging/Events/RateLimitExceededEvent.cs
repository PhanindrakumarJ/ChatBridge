using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBridge.Chat.Infrastructure.Messaging.Events
{
    public class RateLimitExceededEvent
    {
        public string PolicyName { get; set; } = string.Empty;

        public string Endpoint { get; set; } = string.Empty;

        public string ClientIp { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }
    }
}
