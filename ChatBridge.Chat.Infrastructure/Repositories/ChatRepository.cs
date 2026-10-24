using ChatBridge.Chat.Application.DTOS;
using ChatBridge.Chat.Application.Interfaces;
using ChatBridge.Chat.Infrastructure.Data;
using ChatBridge.Chat.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;


namespace ChatBridge.Chat.Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ChatBridgeDbContext _dbContext;

        public ChatRepository(ChatBridgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        async Task<ChatMessageDto> IChatRepository.AddMessageAsync(CreateMessageDto request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                throw new ArgumentException("Message content cannot be empty!",nameof(request));
            }
            if (request.Content.Length > 4000)
            {
                throw new ArgumentException("Message content cannot exceed 4000 characters.", nameof(request));
            }
            await using var transaction =
                await _dbContext.Database.BeginTransactionAsync(
                    System.Data.IsolationLevel.Serializable, cancellationToken);
            var sender = await _dbContext.Users.FirstOrDefaultAsync(
                u => u.Id == request.SenderId, cancellationToken);
            if (sender is null)
            {
                throw new InvalidOperationException("Sender was not found.");
            }

            var isCustomer = string.Equals(
                sender.Role, "Customer", StringComparison.OrdinalIgnoreCase);

            var isVendor = string.Equals(
                sender.Role, "Vendor", StringComparison.OrdinalIgnoreCase);

            if (!isCustomer && !isVendor)
            {
                throw new InvalidOperationException(
                    "The sender must be a customer or vendor.");
            }
            var customer = isCustomer
            ? sender
            : await _dbContext.Users.FirstOrDefaultAsync(
                u => u.Role == "Customer",
                cancellationToken);

            var vendor = isVendor
                ? sender
                : await _dbContext.Users.FirstOrDefaultAsync(
                    u => u.Role == "Vendor",
                    cancellationToken);

            if (customer is null || vendor is null)
            {
                throw new InvalidOperationException(
                    "The seeded customer or vendor was not found.");
            }
            var conversation = await _dbContext.Conversations
            .FirstOrDefaultAsync(
                c => c.CustomerId == customer.Id &&
                     c.VendorId == vendor.Id,
                cancellationToken);

            if (conversation is null)
            {
                conversation = new Conversation
                {
                    CustomerId = customer.Id,
                    VendorId = vendor.Id
                };

                _dbContext.Conversations.Add(conversation);

                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            var message = new Message
            {
                ConversationId = conversation.Id,
                SenderId = sender.Id,
                Content = request.Content
            };

            _dbContext.Messages.Add(message);

            await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            return new ChatMessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                Content = message.Content,
                SentAt = message.SentAt
            };
            //throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<ChatMessageDto>> GetMessagesAsync(
        CancellationToken cancellationToken = default)
        {
            var conversation = await _dbContext.Conversations
                .AsNoTracking()
                .OrderBy(c => c.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (conversation is null)
            {
                return Array.Empty<ChatMessageDto>();
            }

            return await _dbContext.Messages
                .AsNoTracking()
                .Where(m => m.ConversationId == conversation.Id)
                .OrderBy(m => m.SentAt)
                .Select(m => new ChatMessageDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    Content = m.Content,
                    SentAt = m.SentAt
                })
                .ToListAsync(cancellationToken);
        }
    }
}
