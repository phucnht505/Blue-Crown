using BlueCrown.Api.DTOs.Clinics;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueCrown.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClinicController : ControllerBase
    {
        private readonly IClinicService _service;

        public ClinicController(IClinicService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clinics = await _service.GetAllAsync();
            return Ok(clinics);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var clinic = await _service.GetByIdAsync(id);

            if (clinic == null)
                return NotFound(new { message = "Không tìm thấy phòng khám." });

            return Ok(clinic);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] CreateClinicDto dto)
        {
            try
            {
                var clinic = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = clinic.Id }, clinic);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClinicDto dto)
        {
            try
            {
                var result = await _service.UpdateAsync(id, dto);

                if (!result)
                    return NotFound(new { message = "Không tìm thấy phòng khám." });

                return Ok(new { message = "Cập nhật phòng khám thành công." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);

                if (!result)
                    return NotFound(new { message = "Không tìm thấy phòng khám." });

                return Ok(new { message = "Xóa phòng khám thành công." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}