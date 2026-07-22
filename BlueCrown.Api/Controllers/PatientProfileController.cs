using BlueCrown.Api.DTOs.PatientProfiles;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlueCrown.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientProfileController : ControllerBase
    {
        private readonly IPatientProfileService _service;

        public PatientProfileController(IPatientProfileService service)
        {
            _service = service;
        }

        // GET: api/PatientProfile/me
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = GetUserId();

            var profile = await _service.GetMyProfileAsync(userId);

            if (profile == null)
            {
                return NotFound(new
                {
                    message = "Chưa có hồ sơ sức khỏe."
                });
            }

            return Ok(profile);
        }

        // POST: api/PatientProfile
        [HttpPost]
        public async Task<IActionResult> CreateProfile(CreatePatientProfileDto dto)
        {
            var userId = GetUserId();

            await _service.CreateProfileAsync(userId, dto);

            return Ok(new
            {
                message = "Tạo hồ sơ sức khỏe thành công."
            });
        }

        // PUT: api/PatientProfile
        [HttpPut]
        public async Task<IActionResult> UpdateProfile(UpdatePatientProfileDto dto)
        {
            var userId = GetUserId();

            await _service.UpdateProfileAsync(userId, dto);

            return Ok(new
            {
                message = "Cập nhật hồ sơ sức khỏe thành công."
            });
        }

        private Guid GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Token không hợp lệ.");

            return Guid.Parse(userId);
        }
    }
}