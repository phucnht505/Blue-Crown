using BlueCrown.Api.DTOs.DoctorProfiles;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlueCrown.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DoctorProfileController : ControllerBase
    {
        private readonly IDoctorProfileService _service;

        public DoctorProfileController(IDoctorProfileService service)
        {
            _service = service;
        }

        // GET: api/DoctorProfile
        // Patient / Doctor / Admin xem danh sách bác sĩ
        [HttpGet]
        public async Task<ActionResult<List<DoctorProfileDto>>> GetAll()
        {
            var doctors = await _service.GetAllAsync();

            return Ok(doctors);
        }

        // GET: api/DoctorProfile/{id}
        // Xem chi tiết bác sĩ
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DoctorProfileDto>> GetById(Guid id)
        {
            var doctor = await _service.GetByIdAsync(id);

            if (doctor == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy Doctor Profile."
                });
            }

            return Ok(doctor);
        }

        // GET: api/DoctorProfile/user/{userId}
        // Tìm DoctorProfile theo UserId
        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<DoctorProfileDto>> GetByUserId(
            Guid userId)
        {
            var doctor = await _service.GetByUserIdAsync(userId);

            if (doctor == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy Doctor Profile cho User này."
                });
            }

            return Ok(doctor);
        }

        // POST: api/DoctorProfile
        // Doctor tạo hồ sơ của chính mình
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<DoctorProfileDto>> Create(
            [FromBody] CreateDoctorProfileDto dto)
        {
            try
            {
                var userIdClaim =
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new
                    {
                        message = "Không xác định được UserId từ JWT."
                    });
                }

                var doctor =
                    await _service.CreateAsync(userId, dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = doctor.Id },
                    doctor);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // PUT: api/DoctorProfile/{id}
        // Doctor cập nhật hồ sơ của chính mình
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateDoctorProfileDto dto)
        {
            try
            {
                var userIdClaim =
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new
                    {
                        message = "Không xác định được UserId từ JWT."
                    });
                }

                var doctor = await _service.GetByIdAsync(id);

                if (doctor == null)
                {
                    return NotFound(new
                    {
                        message = "Không tìm thấy Doctor Profile."
                    });
                }

                // Chỉ Doctor sở hữu profile mới được cập nhật
                if (doctor.UserId != userId)
                {
                    return Forbid();
                }

                var updated =
                    await _service.UpdateAsync(id, dto);

                if (!updated)
                {
                    return NotFound(new
                    {
                        message = "Không cập nhật được Doctor Profile."
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

        // PUT: api/DoctorProfile/{id}/verify
        // Admin xác minh giấy phép bác sĩ
        [HttpPut("{id:guid}/verify")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VerifyLicense(
            Guid id,
            [FromBody] bool verified)
        {
            var updated =
                await _service.VerifyLicenseAsync(id, verified);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy Doctor Profile."
                });
            }

            return NoContent();
        }
    }
}