using BlueCrown.Api.DTOs.SymptomLogs;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlueCrown.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SymptomLogController : ControllerBase
    {
        private readonly ISymptomLogService _service;

        public SymptomLogController(ISymptomLogService service)
        {
            _service = service;
        }

        [HttpGet("my")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<List<SymptomLogDto>>> GetMyLogs()
        {
            try
            {
                var userId = GetCurrentUserId();
                var patientId = await _service.GetPatientIdByUserIdAsync(userId);

                if (patientId == null)
                    return NotFound(new { message = "Không tìm thấy Patient Profile." });

                var logs = await _service.GetMyLogsAsync(patientId.Value);
                return Ok(logs);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("my/latest")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<SymptomLogDto>> GetLatest()
        {
            try
            {
                var userId = GetCurrentUserId();
                var patientId = await _service.GetPatientIdByUserIdAsync(userId);

                if (patientId == null)
                    return NotFound(new { message = "Không tìm thấy Patient Profile." });

                var log = await _service.GetLatestAsync(patientId.Value);

                if (log == null)
                    return NotFound(new { message = "Chưa có Symptom Log." });

                return Ok(log);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<SymptomLogDto>> GetById(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var patientId = await _service.GetPatientIdByUserIdAsync(userId);

                if (patientId == null)
                    return NotFound(new { message = "Không tìm thấy Patient Profile." });

                var log = await _service.GetByIdAsync(id, patientId.Value);

                if (log == null)
                    return NotFound(new { message = "Không tìm thấy Symptom Log." });

                return Ok(log);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<SymptomLogDto>> Create([FromBody] CreateSymptomLogDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var patientId = await _service.GetPatientIdByUserIdAsync(userId);

                if (patientId == null)
                    return NotFound(new { message = "Không tìm thấy Patient Profile." });

                var log = await _service.CreateAsync(patientId.Value, dto);

                return CreatedAtAction(nameof(GetById), new { id = log.Id }, log);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Không xác định được UserId từ JWT.");

            return userId;
        }
    }
}