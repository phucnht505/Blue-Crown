using BlueCrown.Api.DTOs.Users;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlueCrown.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _service;

        public AccountController(IAccountService service)
        {
            _service = service;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = GetCurrentUserId();

            if (!userId.HasValue)
                return Unauthorized(new { message = "Không xác định được tài khoản đăng nhập." });

            var profile = await _service.GetMyProfileAsync(userId.Value);

            if (profile == null)
                return NotFound(new { message = "Không tìm thấy tài khoản." });

            return Ok(profile);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateAccountProfileDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();

                if (!userId.HasValue)
                    return Unauthorized(new { message = "Không xác định được tài khoản đăng nhập." });

                return Ok(await _service.UpdateMyProfileAsync(userId.Value, dto));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private Guid? GetCurrentUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }
}