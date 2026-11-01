using ChatBridge.Chat.Infrastructure.Messaging.Interfaces;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ChatBridge.Chat.Infrastructure.Messaging.Publishers
{
    public class RabbitMQPublisher : IRabbitMQPublisher
    {
        private readonly IConnection _connection;

        public RabbitMQPublisher(IConnection connection)
        {
            _connection = connection;
        }

        public async Task PublishAsync<T>(T message)
        {
            IChannel channel = await _connection.CreateChannelAsync();

            try
            {
                byte[] body = Encoding.UTF8.GetBytes(
                    JsonSerializer.Serialize(message));

                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: "rate-limit-events",
                    body: body);
            }
            finally
            {
                await channel.CloseAsync();
                await channel.DisposeAsync();
            }
        }
    }
}
