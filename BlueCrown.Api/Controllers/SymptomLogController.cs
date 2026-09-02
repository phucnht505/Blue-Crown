using BlueCrown.Api.DTOs.SymptomLogs;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlueCrown.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SymptomLogController : ControllerBase
    {
        private readonly ISymptomLogService _service;

        public SymptomLogController(ISymptomLogService service)
        {
            _service = service;
        }

        [Authorize(Roles = "patient")]
        [HttpGet("my")]
        public async Task<ActionResult<List<SymptomLogDto>>> GetMyLogs()
        {
            var patientId = await GetCurrentPatientIdAsync();
            return Ok(await _service.GetMyLogsAsync(patientId));
        }

        [Authorize(Roles = "patient")]
        [HttpGet("my/latest")]
        public async Task<ActionResult<SymptomLogDto>> GetLatest()
        {
            var patientId = await GetCurrentPatientIdAsync();
            var log = await _service.GetLatestAsync(patientId);

            if (log == null)
                return NotFound(new { message = "Chưa có Symptom Log." });

            return Ok(log);
        }

        [Authorize(Roles = "patient")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SymptomLogDto>> GetById(Guid id)
        {
            try
            {
                var log = await _service.GetByIdAsync(id, await GetCurrentPatientIdAsync());

                if (log == null)
                    return NotFound(new { message = "Không tìm thấy Symptom Log." });

                return Ok(log);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [Authorize(Roles = "patient")]
        [HttpPost]
        public async Task<ActionResult<SymptomLogDto>> Create([FromBody] CreateSymptomLogDto dto)
        {
            try
            {
                var log = await _service.CreateAsync(await GetCurrentPatientIdAsync(), dto);
                return CreatedAtAction(nameof(GetById), new { id = log.Id }, log);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("analyze")]
        public async Task<ActionResult<SymptomAnalysisDto>> Analyze([FromBody] CreateSymptomLogDto dto)
        {
            try
            {
                Guid? patientId = null;

                if (User.Identity?.IsAuthenticated == true && User.IsInRole("patient"))
                    patientId = await GetCurrentPatientIdAsync();

                var result = await _service.AnalyzeAsync(patientId, dto);
                return Ok(result);
            }
            catch (HttpRequestException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = "Không thể kết nối đến dịch vụ AI." });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = ex.Message });
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

        private async Task<Guid> GetCurrentPatientIdAsync()
        {
            var patientId = await _service.GetPatientIdByUserIdAsync(GetCurrentUserId());

            if (!patientId.HasValue)
                throw new UnauthorizedAccessException("Không tìm thấy Patient Profile.");

            return patientId.Value;
        }
    }
}