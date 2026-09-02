using BlueCrown.Api.DTOs.HealthGoals;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlueCrown.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HealthGoalController : ControllerBase
    {
        private readonly IHealthGoalService _service;

        public HealthGoalController(IHealthGoalService service)
        {
            _service = service;
        }

        [HttpGet("my")]
        [Authorize(Roles = "patient")]
        public async Task<IActionResult> GetMyGoals()
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                return Ok(await _service.GetMyGoalsAsync(userId.Value));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "patient")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var goal = await _service.GetByIdAsync(id, userId.Value);

                if (goal == null)
                    return NotFound(new { message = "Không tìm thấy mục tiêu sức khỏe." });

                return Ok(goal);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "patient")]
        public async Task<IActionResult> Create(CreateHealthGoalDto dto)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var goal = await _service.CreateAsync(userId.Value, dto);
                return CreatedAtAction(nameof(GetById), new { id = goal.Id }, goal);
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

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "patient")]
        public async Task<IActionResult> Update(Guid id, UpdateHealthGoalDto dto)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var result = await _service.UpdateAsync(id, userId.Value, dto);

                if (!result)
                    return NotFound(new { message = "Không tìm thấy mục tiêu sức khỏe." });

                return Ok(new { message = "Cập nhật mục tiêu sức khỏe thành công." });
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

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "patient")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var result = await _service.DeleteAsync(id, userId.Value);

                if (!result)
                    return NotFound(new { message = "Không tìm thấy mục tiêu sức khỏe." });

                return Ok(new { message = "Xóa mục tiêu sức khỏe thành công." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("doctor/patients")]
        [Authorize(Roles = "doctor")]
        public async Task<IActionResult> GetDoctorPatients()
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                return Ok(await _service.GetDoctorPatientsAsync(userId.Value));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("doctor/metric-types")]
        [Authorize(Roles = "doctor")]
        public async Task<IActionResult> GetDoctorMetricTypes()
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                return Ok(await _service.GetDoctorMetricTypesAsync(userId.Value));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("doctor/patient/{patientId:guid}")]
        [Authorize(Roles = "doctor")]
        public async Task<IActionResult> GetDoctorPatientGoals(Guid patientId)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                return Ok(await _service.GetDoctorPatientGoalsAsync(userId.Value, patientId));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("doctor/patient/{patientId:guid}")]
        [Authorize(Roles = "doctor")]
        public async Task<IActionResult> CreateForPatient(Guid patientId, CreateHealthGoalDto dto)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                return Ok(await _service.CreateForPatientAsync(userId.Value, patientId, dto));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
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

        [HttpPut("doctor/patient/{patientId:guid}/{id:guid}")]
        [Authorize(Roles = "doctor")]
        public async Task<IActionResult> UpdateForPatient(Guid patientId, Guid id, UpdateHealthGoalDto dto)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var result = await _service.UpdateForPatientAsync(id, userId.Value, patientId, dto);

                if (!result)
                    return NotFound(new { message = "Không tìm thấy mục tiêu sức khỏe." });

                return Ok(new { message = "Cập nhật mục tiêu sức khỏe thành công." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
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

        [HttpPut("doctor/patient/{patientId:guid}/{id:guid}/cancel")]
        [Authorize(Roles = "doctor")]
        public async Task<IActionResult> CancelForPatient(Guid patientId, Guid id)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var result = await _service.CancelForPatientAsync(id, userId.Value, patientId);

                if (!result)
                    return NotFound(new { message = "Không tìm thấy mục tiêu sức khỏe." });

                return Ok(new { message = "Đã hủy mục tiêu sức khỏe." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private Guid? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim?.Value, out var id) ? id : null;
        }
    }
}