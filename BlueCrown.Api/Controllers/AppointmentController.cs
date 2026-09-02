using BlueCrown.Api.DTOs.Appointments;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlueCrown.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet("my")]
        [Authorize(Roles = "patient")]
        public async Task<ActionResult<List<AppointmentDto>>> GetMyAppointments()
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                return Ok(await _appointmentService.GetMyAppointmentsAsync(userId.Value));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("doctors")]
        [Authorize(Roles = "patient")]
        public async Task<ActionResult<List<AppointmentDoctorDto>>> GetDoctors()
        {
            return Ok(await _appointmentService.GetBookableDoctorsAsync());
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "patient")]
        public async Task<ActionResult<AppointmentDto>> GetById(Guid id)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var appointment = await _appointmentService.GetByIdAsync(id, userId.Value);

                if (appointment == null)
                    return NotFound(new { message = "Không tìm thấy lịch khám." });

                return Ok(appointment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "patient")]
        public async Task<ActionResult<AppointmentDto>> Create([FromBody] CreateAppointmentDto dto)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var appointment = await _appointmentService.CreateAsync(userId.Value, dto);

                return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
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

                var deleted = await _appointmentService.DeleteAsync(id, userId.Value);

                if (!deleted)
                    return NotFound(new { message = "Không tìm thấy lịch khám." });

                return Ok(new { message = "Hủy lịch khám thành công." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // THÊM MỚI: Doctor xem lịch được đặt với chính mình.
        [HttpGet("doctor/my")]
        [Authorize(Roles = "doctor")]
        public async Task<ActionResult<List<AppointmentDto>>> GetDoctorAppointments()
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                return Ok(await _appointmentService.GetDoctorAppointmentsAsync(userId.Value));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // THÊM MỚI: Doctor xác nhận, từ chối hoặc hoàn thành lịch của chính mình.
        [HttpPut("doctor/{id:guid}/status")]
        [Authorize(Roles = "doctor")]
        public async Task<ActionResult<AppointmentDto>> UpdateDoctorStatus(Guid id, [FromBody] UpdateAppointmentStatusDto dto)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var appointment = await _appointmentService.UpdateDoctorStatusAsync(id, userId.Value, dto);

                if (appointment == null)
                    return NotFound(new { message = "Không tìm thấy lịch khám." });

                return Ok(appointment);
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