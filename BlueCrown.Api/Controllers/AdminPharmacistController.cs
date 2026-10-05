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
    public class AdminPharmacistController : ControllerBase
    {
        private readonly IAdminPharmacistService _service;

        public AdminPharmacistController(IAdminPharmacistService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] string? status)
        {
            try
            {
                var pharmacists = await _service.GetAllAsync(search, status);
                return Ok(pharmacists);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var pharmacist = await _service.GetByIdAsync(id);

                if (pharmacist == null)
                    return NotFound(new { message = "Không tìm thấy dược sĩ." });

                return Ok(pharmacist);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] AdminPharmacistCreateDto dto)
        {
            try
            {
                var pharmacist = await _service.CreateAsync(dto);
                return Ok(pharmacist);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] AdminPharmacistUpdateDto dto)
        {
            try
            {
                var pharmacist = await _service.UpdateAsync(
                    id,
                    dto,
                    GetCurrentUserId());

                return Ok(pharmacist);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(
            Guid id,
            [FromBody] UpdateUserStatusDto dto)
        {
            try
            {
                var pharmacist = await _service.UpdateStatusAsync(
                    id,
                    dto,
                    GetCurrentUserId());

                return Ok(pharmacist);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id:guid}/hard")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var message = await _service.DeleteAsync(
                    id,
                    GetCurrentUserId());

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
                throw new UnauthorizedAccessException(
                    "Không xác định được tài khoản đăng nhập.");

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