using BlueCrown.Api.DTOs.HealthMetrics;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlueCrown.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Patient")]
    public class HealthMetricController : ControllerBase
    {
        private readonly IHealthMetricService _service;

        public HealthMetricController(IHealthMetricService service)
        {
            _service = service;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyMetrics()
        {
            var patientId = GetPatientId();

            if (patientId == null)
                return Unauthorized(new { message = "Không xác định được Patient." });

            return Ok(await _service.GetMyMetricsAsync(patientId.Value));
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatest()
        {
            var patientId = GetPatientId();

            if (patientId == null)
                return Unauthorized(new { message = "Không xác định được Patient." });

            var metric = await _service.GetLatestAsync(patientId.Value);

            if (metric == null)
                return NotFound(new { message = "Chưa có dữ liệu sức khỏe." });

            return Ok(metric);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var patientId = GetPatientId();

            if (patientId == null)
                return Unauthorized(new { message = "Không xác định được Patient." });

            var metric = await _service.GetByIdAsync(id, patientId.Value);

            if (metric == null)
                return NotFound(new { message = "Không tìm thấy HealthMetric." });

            return Ok(metric);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateHealthMetricDto dto)
        {
            try
            {
                var patientId = GetPatientId();

                if (patientId == null)
                    return Unauthorized(new { message = "Không xác định được Patient." });

                var metric = await _service.CreateAsync(patientId.Value, dto);

                return CreatedAtAction(nameof(GetById), new { id = metric.Id }, metric);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private Guid? GetPatientId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim?.Value, out var id) ? id : null;
        }
    }
}