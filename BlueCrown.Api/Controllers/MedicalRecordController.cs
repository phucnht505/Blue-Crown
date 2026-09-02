using BlueCrown.Api.DTOs.MedicalRecords;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlueCrown.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordService _service;

        public MedicalRecordController(IMedicalRecordService service)
        {
            _service = service;
        }

        [HttpGet("patient/my")]
        [Authorize(Roles = "patient")]
        public async Task<ActionResult<List<MedicalRecordDto>>> GetPatientRecords()
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                return Ok(await _service.GetPatientRecordsAsync(userId.Value));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("patient/{id:guid}")]
        [Authorize(Roles = "patient")]
        public async Task<ActionResult<MedicalRecordDto>> GetPatientRecordById(Guid id)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var record = await _service.GetPatientRecordByIdAsync(id, userId.Value);

                if (record == null)
                    return NotFound(new { message = "Không tìm thấy hồ sơ bệnh án." });

                return Ok(record);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("doctor/my")]
        [Authorize(Roles = "doctor")]
        public async Task<ActionResult<List<MedicalRecordDto>>> GetDoctorRecords()
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                return Ok(await _service.GetDoctorRecordsAsync(userId.Value));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("doctor/{id:guid}")]
        [Authorize(Roles = "doctor")]
        public async Task<ActionResult<MedicalRecordDto>> GetDoctorRecordById(Guid id)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var record = await _service.GetDoctorRecordByIdAsync(id, userId.Value);

                if (record == null)
                    return NotFound(new { message = "Không tìm thấy hồ sơ bệnh án." });

                return Ok(record);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("doctor/appointment/{appointmentId:guid}")]
        [Authorize(Roles = "doctor")]
        public async Task<ActionResult<MedicalRecordDto>> GetDoctorRecordByAppointment(Guid appointmentId)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var record = await _service.GetDoctorRecordByAppointmentAsync(appointmentId, userId.Value);

                if (record == null)
                    return NotFound(new { message = "Lịch khám này chưa có hồ sơ bệnh án." });

                return Ok(record);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "doctor")]
        public async Task<ActionResult<MedicalRecordDto>> Create([FromBody] CreateMedicalRecordDto dto)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var record = await _service.CreateAsync(userId.Value, dto);

                return CreatedAtAction(nameof(GetDoctorRecordById), new { id = record.Id }, record);
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
        [Authorize(Roles = "doctor")]
        public async Task<ActionResult<MedicalRecordDto>> Update(Guid id, [FromBody] UpdateMedicalRecordDto dto)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var record = await _service.UpdateAsync(id, userId.Value, dto);

                if (record == null)
                    return NotFound(new { message = "Không tìm thấy hồ sơ bệnh án." });

                return Ok(record);
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