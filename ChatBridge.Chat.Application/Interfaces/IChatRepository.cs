using ChatBridge.Chat.Application.DTOS;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBridge.Chat.Application.Interfaces
{
    public interface IChatRepository
    {
        Task<IReadOnlyList<ChatMessageDto>>GetMessagesAsync(
            CancellationToken cancellationToken = default);

        Task<ChatMessageDto> AddMessageAsync(
            CreateMessageDto request,
            CancellationToken cancellationToken = default);
    }
}
