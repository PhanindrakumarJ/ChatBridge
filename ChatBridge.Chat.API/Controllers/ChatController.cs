using ChatBridge.Chat.API.Hubs;
using ChatBridge.Chat.Application.DTOS;
using ChatBridge.Chat.Application.Interfaces;
using ChatBridge.Chat.Infrastructure.GlobalTimer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;

namespace ChatBridge.Chat.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatRepository _chatRepository;
        private readonly GlobalMessageCooldown _cooldown;

        //SignalR dependency
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IChatRepository chatRepository, IHubContext<ChatHub> hubContext, GlobalMessageCooldown cooldown)
        {
            _chatRepository = chatRepository;
            _hubContext = hubContext;
            _cooldown = cooldown;
        }

        [HttpGet("messages")]
        public async Task<ActionResult<IReadOnlyList<ChatMessageDto>>>
            GetMessages(CancellationToken cancellationToken)
        {
            try
            {
            var messages = await _chatRepository.GetMessagesAsync(
                cancellationToken);

            return Ok(messages);

            }
            catch(Exception ex)
            {
                throw;
            }
        }




        [HttpPost("messages")]
        [EnableRateLimiting("MessageRateLimit")]
        public async Task<ActionResult<ChatMessageDto>> SendMessage(
            [FromBody] CreateMessageDto request,
            CancellationToken cancellationToken)
        {
            if (!_cooldown.TryReserveSend(
                out DateTime nextAllowedSendUtc,
                out bool sendInProgress))
            {
                return StatusCode(StatusCodes.Status429TooManyRequests, new
                {
                    message = sendInProgress
                        ? "Another message is being processed. Please retry shortly."
                        : "Please wait for the global cooldown to expire.",
                    nextAllowedSendUtc,
                    sendInProgress
                });
            }

            bool sendCompleted = false;

            try
            {
                var message = await _chatRepository.AddMessageAsync(
                    request,
                    cancellationToken);

                nextAllowedSendUtc = _cooldown.CompleteSend();
                sendCompleted = true;

                await _hubContext.Clients.All.SendAsync(
                    "ReceiveMessage",
                    message,
                    cancellationToken);

                await _hubContext.Clients.All.SendAsync(
                    "CooldownUpdated",
                    nextAllowedSendUtc,
                    cancellationToken);

                return Ok(message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            finally
            {
                if (!sendCompleted)
                {
                    _cooldown.CancelSend();
                }
            }
        }


    }
}
