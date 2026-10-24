using ChatBridge.Chat.Application.DTOS;
using ChatBridge.Chat.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ChatBridge.Chat.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatRepository _chatRepository;

        public ChatController(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
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
            try
            {
                var message = await _chatRepository.AddMessageAsync(
                    request,
                    cancellationToken);

                return Ok(message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
