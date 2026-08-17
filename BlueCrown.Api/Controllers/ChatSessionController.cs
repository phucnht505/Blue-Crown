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

        // =========================================================
        // GET: api/ChatSession/my
        // Patient xem các phiên chat của mình
        // =========================================================
        [HttpGet("my")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<List<ChatSessionDto>>> GetMySessions()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new
                {
                    message = "Không xác định được UserId từ JWT."
                });
            }

            var patientId = await _service.GetPatientIdByUserIdAsync(userId);

            if (patientId == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy Patient Profile."
                });
            }

            var sessions = await _service.GetMySessionsAsync(patientId.Value);

            return Ok(sessions);
        }

        // =========================================================
        // GET: api/ChatSession/doctor
        // Doctor xem các phiên chat được gán cho mình
        // =========================================================
        [HttpGet("doctor")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<List<ChatSessionDto>>> GetDoctorSessions()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new
                {
                    message = "Không xác định được UserId từ JWT."
                });
            }

            var doctorId = await _service.GetDoctorIdByUserIdAsync(userId);

            if (doctorId == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy Doctor Profile."
                });
            }

            var sessions = await _service.GetDoctorSessionsAsync(doctorId.Value);

            return Ok(sessions);
        }

        // =========================================================
        // GET: api/ChatSession/{id}
        // Xem chi tiết ChatSession
        // =========================================================
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ChatSessionDto>> GetById(Guid id)
        {
            var session = await _service.GetByIdAsync(id);

            if (session == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy Chat Session."
                });
            }

            return Ok(session);
        }

        // =========================================================
        // POST: api/ChatSession
        // Patient tạo ChatSession
        // =========================================================
        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<ChatSessionDto>> Create([FromBody] CreateChatSessionDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new
                    {
                        message = "Không xác định được UserId từ JWT."
                    });
                }

                var patientId = await _service.GetPatientIdByUserIdAsync(userId);

                if (patientId == null)
                {
                    return NotFound(new
                    {
                        message = "Không tìm thấy Patient Profile."
                    });
                }

                var session = await _service.CreateAsync(patientId.Value, dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = session.Id },
                    session);
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
        // PUT: api/ChatSession/{id}/assign-doctor
        // Doctor nhận/gán mình vào ChatSession
        // =========================================================
        [HttpPut("{id:guid}/assign-doctor")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> AssignDoctor(Guid id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new
                    {
                        message = "Không xác định được UserId từ JWT."
                    });
                }

                var doctorId = await _service.GetDoctorIdByUserIdAsync(userId);

                if (doctorId == null)
                {
                    return NotFound(new
                    {
                        message = "Không tìm thấy Doctor Profile."
                    });
                }

                var result = await _service.AssignDoctorAsync(id, doctorId.Value);

                if (!result)
                {
                    return NotFound(new
                    {
                        message = "Không tìm thấy Chat Session."
                    });
                }

                return NoContent();
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
        // PUT: api/ChatSession/{id}/status
        // Cập nhật trạng thái ChatSession
        // =========================================================
        [HttpPut("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateChatSessionStatusDto dto)
        {
            try
            {
                var result = await _service.UpdateStatusAsync(id, dto);

                if (!result)
                {
                    return NotFound(new
                    {
                        message = "Không tìm thấy Chat Session."
                    });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}