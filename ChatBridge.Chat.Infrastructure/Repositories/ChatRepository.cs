using ChatBridge.Chat.Application.DTOS;
using ChatBridge.Chat.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;


namespace ChatBridge.Chat.Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        Task<ChatMessageDto> IChatRepository.AddMessageAsync(CreateMessageDto request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<IReadOnlyList<ChatMessageDto>> IChatRepository.GetMessagesAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
