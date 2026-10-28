using ChatBridge.Chat.Infrastructure.GlobalTimer;
using Microsoft.AspNetCore.SignalR;

namespace ChatBridge.Chat.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly GlobalMessageCooldown _cooldown;

        public ChatHub(GlobalMessageCooldown cooldown)
        {
            _cooldown = cooldown;
        }
        public async Task SendTestMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveTestMessage", message);
        }
        public async Task SendChatMessage(object message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
        public async Task GetCooldown()
        {
            DateTime nextAllowedSendUtc =
                _cooldown.GetNextAllowedSendUtc();

            await Clients.Caller.SendAsync(
                "CooldownUpdated",
                nextAllowedSendUtc);
        }
    }
}
