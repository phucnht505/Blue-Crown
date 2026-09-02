using BlueCrown.Api.DTOs.ChatMessages;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlueCrown.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "patient,doctor")]
    public class ChatMessageController : ControllerBase
    {
        private readonly IChatMessageService _service;

        public ChatMessageController(IChatMessageService service)
        {
            _service = service;
        }

        // =========================================================
        // GET: api/ChatMessage/session/{sessionId}
        // Patient / Doctor xem tin nhắn trong ChatSession
        // =========================================================
        [HttpGet("session/{sessionId:guid}")]
        public async Task<ActionResult<List<ChatMessageDto>>> GetBySessionId(
            Guid sessionId)
        {
            try
            {
                var userId = GetCurrentUserId();

                var messages =
                    await _service.GetBySessionIdAsync(
                        sessionId,
                        userId);

                return Ok(messages);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        // =========================================================
        // GET: api/ChatMessage/{id}
        // Xem một tin nhắn
        // =========================================================
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ChatMessageDto>> GetById(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();

                var message =
                    await _service.GetByIdAsync(
                        id,
                        userId);

                if (message == null)
                {
                    return NotFound(new
                    {
                        message = "Không tìm thấy Chat Message."
                    });
                }

                return Ok(message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        // =========================================================
        // POST: api/ChatMessage
        // Patient / Doctor gửi tin nhắn
        // =========================================================
        [HttpPost]
        public async Task<ActionResult<ChatMessageDto>> Create(
            [FromBody] CreateChatMessageDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();

                var message =
                    await _service.CreateAsync(
                        userId,
                        dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = message.Id },
                    message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // =========================================================
        // PUT: api/ChatMessage/{id}/read
        // Người nhận đánh dấu tin nhắn đã đọc
        // =========================================================
        [HttpPut("{id:guid}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();

                var result =
                    await _service.MarkAsReadAsync(
                        id,
                        userId);

                if (!result)
                {
                    return NotFound(new
                    {
                        message = "Không tìm thấy Chat Message."
                    });
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // =========================================================
        // Helper
        // =========================================================
        private Guid GetCurrentUserId()
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException(
                    "Không xác định được UserId từ JWT.");
            }

            return userId;
        }
    }
}