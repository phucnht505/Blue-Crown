using BlueCrown.Api.DTOs.Users;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlueCrown.Api.Controllers
{
    [Authorize(Roles = "admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] string? search, [FromQuery] string? role, [FromQuery] string? status)
        {
            try
            {
                var users = await _userService.GetAllUsersAsync(search, role, status);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound(new { message = "Không tìm thấy người dùng." });

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserDto dto)
        {
            try
            {
                var user = await _userService.CreateUserByAdminAsync(dto);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] AdminUpdateUserDto dto)
        {
            try
            {
                var user = await _userService.UpdateUserByAdminAsync(id, dto, GetCurrentUserId());
                return Ok(user);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateUserStatusDto dto)
        {
            try
            {
                var user = await _userService.UpdateUserStatusAsync(id, dto, GetCurrentUserId());
                return Ok(user);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var message = await _userService.DeleteUserByAdminAsync(id, GetCurrentUserId());
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private Guid GetCurrentUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(value, out var id))
                throw new UnauthorizedAccessException("Không xác định được tài khoản đăng nhập.");

            return id;
        }

        private IActionResult HandleException(Exception ex)
        {
            if (ex is KeyNotFoundException)
                return NotFound(new { message = ex.Message });

            if (ex is UnauthorizedAccessException)
                return Unauthorized(new { message = ex.Message });

            return BadRequest(new { message = ex.Message });
        }
    }
}