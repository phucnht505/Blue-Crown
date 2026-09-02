using BlueCrown.Api.DTOs.ChatSessions;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlueCrown.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatSessionController : ControllerBase
    {
        private readonly IChatSessionService _service;

        public ChatSessionController(IChatSessionService service)
        {
            _service = service;
        }

        [HttpGet("my")]
        [Authorize(Roles = "patient")]
        public async Task<ActionResult<List<ChatSessionDto>>> GetMySessions()
        {
            var patientId = await _service.GetPatientIdByUserIdAsync(GetCurrentUserId());

            if (patientId == null)
                return NotFound(new { message = "Không tìm thấy Patient Profile." });

            return Ok(await _service.GetMySessionsAsync(patientId.Value));
        }

        [HttpGet("doctor")]
        [Authorize(Roles = "doctor")]
        public async Task<ActionResult<List<ChatSessionDto>>> GetDoctorSessions()
        {
            var doctorId = await _service.GetDoctorIdByUserIdAsync(GetCurrentUserId());

            if (doctorId == null)
                return NotFound(new { message = "Không tìm thấy Doctor Profile." });

            return Ok(await _service.GetDoctorSessionsAsync(doctorId.Value));
        }

        [HttpGet("doctor/available")]
        [Authorize(Roles = "doctor")]
        public async Task<ActionResult<List<ChatSessionDto>>> GetAvailableSessions()
        {
            return Ok(await _service.GetAvailableSessionsAsync());
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "patient,doctor")]
        public async Task<ActionResult<ChatSessionDto>> GetById(Guid id)
        {
            try
            {
                var session = await _service.GetByIdAsync(id, GetCurrentUserId());

                if (session == null)
                    return NotFound(new { message = "Không tìm thấy Chat Session." });

                return Ok(session);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Authorize(Roles = "patient")]
        public async Task<ActionResult<ChatSessionDto>> Create([FromBody] CreateChatSessionDto dto)
        {
            try
            {
                var patientId = await _service.GetPatientIdByUserIdAsync(GetCurrentUserId());

                if (patientId == null)
                    return NotFound(new { message = "Không tìm thấy Patient Profile." });

                var session = await _service.CreateAsync(patientId.Value, dto);
                return CreatedAtAction(nameof(GetById), new { id = session.Id }, session);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:guid}/assign-doctor")]
        [Authorize(Roles = "doctor")]
        public async Task<IActionResult> AssignDoctor(Guid id)
        {
            try
            {
                var doctorId = await _service.GetDoctorIdByUserIdAsync(GetCurrentUserId());

                if (doctorId == null)
                    return NotFound(new { message = "Không tìm thấy Doctor Profile." });

                var result = await _service.AssignDoctorAsync(id, doctorId.Value);

                if (!result)
                    return NotFound(new { message = "Không tìm thấy Chat Session." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = "patient,doctor")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateChatSessionStatusDto dto)
        {
            try
            {
                var result = await _service.UpdateStatusAsync(id, GetCurrentUserId(), dto);

                if (!result)
                    return NotFound(new { message = "Không tìm thấy Chat Session." });

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private Guid GetCurrentUserId()
        {
            var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(value, out var userId))
                throw new UnauthorizedAccessException("Không xác định được UserId từ JWT.");

            return userId;
        }
    }
}