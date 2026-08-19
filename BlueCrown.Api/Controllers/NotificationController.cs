using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlueCrown.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationController(INotificationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            return Ok(await _service.GetMyNotificationsAsync(userId.Value));
        }

        [HttpGet("unread")]
        public async Task<IActionResult> GetMyUnreadNotifications()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            return Ok(await _service.GetMyUnreadNotificationsAsync(userId.Value));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            try
            {
                var notification = await _service.GetByIdAsync(id, userId.Value);

                if (notification == null)
                    return NotFound(new
                    {
                        message = "Không tìm thấy notification."
                    });

                return Ok(notification);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPut("{id:guid}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            try
            {
                var result = await _service.MarkAsReadAsync(id, userId.Value);

                if (!result)
                    return NotFound(new
                    {
                        message = "Không tìm thấy notification."
                    });

                return Ok(new
                {
                    message = "Đã đánh dấu notification là đã đọc."
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        private Guid? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
                return null;

            return Guid.TryParse(claim.Value, out var userId) ? userId : null;
        }
    }
}