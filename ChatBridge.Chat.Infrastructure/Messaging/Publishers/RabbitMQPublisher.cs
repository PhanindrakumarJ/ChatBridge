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
                await channel.QueueDeclareAsync(
                    queue: "rate-limit-events",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                byte[] body = Encoding.UTF8.GetBytes(
                    JsonSerializer.Serialize(message));

                BasicProperties properties = new BasicProperties
                {
                    Persistent = true
                };

                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: "rate-limit-events",
                    mandatory: true,
                    basicProperties: properties,
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
