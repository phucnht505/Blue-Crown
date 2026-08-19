using BlueCrown.Api.DTOs.HealthGoals;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlueCrown.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Patient")]
    public class HealthGoalController : ControllerBase
    {
        private readonly IHealthGoalService _service;

        public HealthGoalController(IHealthGoalService service)
        {
            _service = service;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyGoals()
        {
            var patientId = GetPatientId();

            if (patientId == null)
                return Unauthorized(new { message = "Không xác định được Patient." });

            return Ok(await _service.GetMyGoalsAsync(patientId.Value));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var patientId = GetPatientId();

            if (patientId == null)
                return Unauthorized(new { message = "Không xác định được Patient." });

            var goal = await _service.GetByIdAsync(id, patientId.Value);

            if (goal == null)
                return NotFound(new { message = "Không tìm thấy HealthGoal." });

            return Ok(goal);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateHealthGoalDto dto)
        {
            try
            {
                var patientId = GetPatientId();

                if (patientId == null)
                    return Unauthorized(new { message = "Không xác định được Patient." });

                var goal = await _service.CreateAsync(patientId.Value, dto);

                return CreatedAtAction(nameof(GetById), new { id = goal.Id }, goal);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateHealthGoalDto dto)
        {
            try
            {
                var patientId = GetPatientId();

                if (patientId == null)
                    return Unauthorized(new { message = "Không xác định được Patient." });

                var result = await _service.UpdateAsync(id, patientId.Value, dto);

                if (!result)
                    return NotFound(new { message = "Không tìm thấy HealthGoal." });

                return Ok(new { message = "Cập nhật HealthGoal thành công." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var patientId = GetPatientId();

            if (patientId == null)
                return Unauthorized(new { message = "Không xác định được Patient." });

            var result = await _service.DeleteAsync(id, patientId.Value);

            if (!result)
                return NotFound(new { message = "Không tìm thấy HealthGoal." });

            return Ok(new { message = "Xóa HealthGoal thành công." });
        }

        private Guid? GetPatientId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            return Guid.TryParse(claim?.Value, out var id) ? id : null;
        }
    }
}