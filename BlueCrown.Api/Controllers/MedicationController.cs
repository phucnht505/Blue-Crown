using BlueCrown.Api.DTOs.Medications;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueCrown.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicationController : ControllerBase
    {
        private readonly IMedicationService _service;

        public MedicationController(IMedicationService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "doctor,pharmacist,admin")]
        public async Task<ActionResult<List<MedicationDto>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "doctor,pharmacist,admin")]
        public async Task<ActionResult<MedicationDto>> GetById(Guid id)
        {
            var medication = await _service.GetByIdAsync(id);

            if (medication == null)
                return NotFound(new { message = "Không tìm thấy Medication." });

            return Ok(medication);
        }

        [HttpPost]
        [Authorize(Roles = "pharmacist")]
        public async Task<ActionResult<MedicationDto>> Create([FromBody] CreateMedicationDto dto)
        {
            try
            {
                var medication = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = medication.Id }, medication);
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
        [Authorize(Roles = "pharmacist")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMedicationDto dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);

                if (!updated)
                    return NotFound(new { message = "Không tìm thấy Medication." });

                return Ok(new { message = "Cập nhật Medication thành công." });
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
        [Authorize(Roles = "pharmacist")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);

                if (!deleted)
                    return NotFound(new { message = "Không tìm thấy Medication." });

                return Ok(new { message = "Xóa Medication thành công." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}