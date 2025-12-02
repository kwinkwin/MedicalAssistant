using MedicalAssistant.Application.DTOs;
using MedicalAssistant.Application.Interfaces;
using MedicalAssistant.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAssistant.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IChatService _chatService;

        public ChatController(ILogger<ChatController> logger,
                              IChatService chatService)
        {
            _logger = logger;
            _chatService = chatService;
        }

        [HttpGet("GetConversation")]
        public async Task<ActionResult<List<ConversationDto>>> GetConversationsAsync()
        {
            try
            {
                var result = await _chatService.GetConversationsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Message: {ex.Message} \n InnerException: {ex.InnerException}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetMessagesByConversationId")]
        public async Task<ActionResult<List<MessageDto>>> GetMessagesByConversationIdAsync(Guid idConversation)
        {
            try
            {
                var result = await _chatService.GetMessagesByConversationIdAsync(idConversation);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Message: {ex.Message} \n InnerException: {ex.InnerException}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("SendMessage")]
        public async Task<ActionResult<MessageDto>> SendMessageAsync(SendMessageRequest request)
        {
            try
            {
                var result = await _chatService.SendMessageAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Message: {ex.Message} \n InnerException: {ex.InnerException}");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteConversation")]
        public async Task<ActionResult<string>> DeleteConversationAsync(Guid idConversation)
        {
            try
            {
                var result = await _chatService.DeleteConversationAsync(idConversation);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Message: {ex.Message} \n InnerException: {ex.InnerException}");
                return BadRequest(ex.Message);
            }

        }
    }
}
