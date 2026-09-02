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

        [HttpGet]
        [Authorize(Roles = "patient,doctor,admin")]
        public async Task<ActionResult<List<DoctorProfileDto>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "patient,doctor,admin")]
        public async Task<ActionResult<DoctorProfileDto>> GetById(Guid id)
        {
            var doctor = await _service.GetByIdAsync(id);

            if (doctor == null)
                return NotFound(new { message = "Không tìm thấy hồ sơ bác sĩ." });

            return Ok(doctor);
        }

        [HttpGet("user/{userId:guid}")]
        [Authorize(Roles = "doctor,admin")]
        public async Task<ActionResult<DoctorProfileDto>> GetByUserId(Guid userId)
        {
            var doctor = await _service.GetByUserIdAsync(userId);

            if (doctor == null)
                return NotFound(new { message = "Không tìm thấy hồ sơ bác sĩ cho User này." });

            return Ok(doctor);
        }

        [HttpGet("me")]
        [Authorize(Roles = "doctor")]
        public async Task<ActionResult<DoctorProfileDto>> GetMyProfile()
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized(new { message = "Không xác định được người dùng." });

            var doctor = await _service.GetByUserIdAsync(userId.Value);

            if (doctor == null)
                return NotFound(new { message = "Bạn chưa có hồ sơ bác sĩ." });

            return Ok(doctor);
        }

        [HttpPost]
        [Authorize(Roles = "doctor")]
        public async Task<ActionResult<DoctorProfileDto>> Create([FromBody] CreateDoctorProfileDto dto)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var doctor = await _service.CreateAsync(userId.Value, dto);
                return CreatedAtAction(nameof(GetById), new { id = doctor.Id }, doctor);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "doctor")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDoctorProfileDto dto)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var doctor = await _service.GetByIdAsync(id);

                if (doctor == null)
                    return NotFound(new { message = "Không tìm thấy hồ sơ bác sĩ." });

                if (doctor.UserId != userId.Value)
                    return Forbid();

                var updated = await _service.UpdateAsync(id, dto);

                if (!updated)
                    return NotFound(new { message = "Không cập nhật được hồ sơ bác sĩ." });

                return Ok(new { message = "Cập nhật hồ sơ bác sĩ thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:guid}/verify")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> VerifyLicense(Guid id, [FromBody] bool verified)
        {
            var updated = await _service.VerifyLicenseAsync(id, verified);

            if (!updated)
                return NotFound(new { message = "Không tìm thấy hồ sơ bác sĩ." });

            return Ok(new { message = verified ? "Đã xác minh giấy phép bác sĩ." : "Đã hủy xác minh giấy phép bác sĩ." });
        }

        [HttpGet("admin")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<DoctorProfileDto>>> AdminGetAll([FromQuery] string? search, [FromQuery] string? specialty, [FromQuery] string? status)
        {
            return Ok(await _service.GetAllAsync(search, specialty, status));
        }

        [HttpGet("admin/meta")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<AdminDoctorMetaDto>> AdminGetMeta()
        {
            return Ok(await _service.GetAdminMetaAsync());
        }

        [HttpGet("admin/{id:guid}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminGetById(Guid id)
        {
            var doctor = await _service.GetByIdAsync(id);

            if (doctor == null)
                return NotFound(new { message = "Không tìm thấy bác sĩ." });

            return Ok(doctor);
        }

        [HttpPost("admin")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminCreate([FromBody] AdminCreateDoctorDto dto)
        {
            try
            {
                return Ok(await _service.AdminCreateAsync(dto));
            }
            catch (Exception ex)
            {
                return HandleAdminException(ex);
            }
        }

        [HttpPut("admin/{id:guid}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminUpdate(Guid id, [FromBody] AdminUpdateDoctorDto dto)
        {
            try
            {
                return Ok(await _service.AdminUpdateAsync(id, dto));
            }
            catch (Exception ex)
            {
                return HandleAdminException(ex);
            }
        }

        [HttpPatch("admin/{id:guid}/status")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminUpdateStatus(Guid id, [FromBody] UpdateDoctorStatusDto dto)
        {
            try
            {
                return Ok(await _service.AdminUpdateStatusAsync(id, dto));
            }
            catch (Exception ex)
            {
                return HandleAdminException(ex);
            }
        }

        [HttpDelete("admin/{id:guid}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminDeactivate(Guid id)
        {
            try
            {
                var message = await _service.AdminDeactivateAsync(id);
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return HandleAdminException(ex);
            }
        }

        private Guid? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim?.Value, out var id) ? id : null;
        }

        private IActionResult HandleAdminException(Exception ex)
        {
            if (ex is KeyNotFoundException)
                return NotFound(new { message = ex.Message });

            return BadRequest(new { message = ex.Message });
        }
    }
}