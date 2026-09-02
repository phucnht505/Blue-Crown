using BlueCrown.Api.DTOs.HealthMetrics;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlueCrown.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "patient")]
    public class HealthMetricController : ControllerBase
    {
        private readonly IHealthMetricService _service;

        public HealthMetricController(IHealthMetricService service)
        {
            _service = service;
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetMetricTypes()
        {
            return Ok(await _service.GetMetricTypesAsync());
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyMetrics()
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                {
                    return Unauthorized(new
                    {
                        message = "Không xác định được người dùng."
                    });
                }

                return Ok(
                    await _service.GetMyMetricsAsync(userId.Value)
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatest()
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                {
                    return Unauthorized(new
                    {
                        message = "Không xác định được người dùng."
                    });
                }

                var metric = await _service.GetLatestAsync(
                    userId.Value
                );

                if (metric == null)
                {
                    return NotFound(new
                    {
                        message = "Chưa có dữ liệu sức khỏe."
                    });
                }

                return Ok(metric);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                {
                    return Unauthorized(new
                    {
                        message = "Không xác định được người dùng."
                    });
                }

                var metric = await _service.GetByIdAsync(
                    id,
                    userId.Value
                );

                if (metric == null)
                {
                    return NotFound(new
                    {
                        message = "Không tìm thấy chỉ số sức khỏe."
                    });
                }

                return Ok(metric);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateHealthMetricDto dto)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                {
                    return Unauthorized(new
                    {
                        message = "Không xác định được người dùng."
                    });
                }

                var metric = await _service.CreateAsync(
                    userId.Value,
                    dto
                );

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = metric.Id },
                    metric
                );
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

        private Guid? GetUserId()
        {
            var claim = User.FindFirst(
                ClaimTypes.NameIdentifier
            );

            return Guid.TryParse(
                claim?.Value,
                out var id
            )
                ? id
                : null;
        }
    }
}