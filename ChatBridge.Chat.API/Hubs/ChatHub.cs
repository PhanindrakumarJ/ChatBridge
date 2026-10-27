using Microsoft.AspNetCore.SignalR;

namespace ChatBridge.Chat.API.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendTestMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveTestMessage", message);
        }
    }
}
