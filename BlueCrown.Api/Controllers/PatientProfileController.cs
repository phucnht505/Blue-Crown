using BlueCrown.Api.DTOs.PatientProfiles;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlueCrown.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientProfileController : ControllerBase
    {
        private readonly IPatientProfileService _service;

        public PatientProfileController(IPatientProfileService service)
        {
            _service = service;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            Guid userId = GetCurrentUserId();

            var profile = await _service.GetMyProfileAsync(userId);

            if (profile == null)
                return NotFound(new { message = "Chưa có hồ sơ sức khỏe." });

            return Ok(profile);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProfile(CreatePatientProfileDto dto)
        {
            Guid userId = GetCurrentUserId();

            await _service.CreateProfileAsync(userId, dto);

            return Ok(new
            {
                message = "Tạo hồ sơ thành công."
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile(UpdatePatientProfileDto dto)
        {
            Guid userId = GetCurrentUserId();

            await _service.UpdateProfileAsync(userId, dto);

            return Ok(new
            {
                message = "Cập nhật hồ sơ thành công."
            });
        }

        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Token không hợp lệ.");

            return Guid.Parse(userId);
        }
    }
}