using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBridge.Chat.Infrastructure.Messaging.Interfaces
{
    public interface IRabbitMQPublisher
    {
        Task PublishAsync<T>(T message);
    }
}
