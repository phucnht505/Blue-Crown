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

        // =========================================================
        // GET: api/Medication
        // User đã đăng nhập có thể xem danh sách thuốc
        // =========================================================
        [HttpGet]
        public async Task<ActionResult<List<MedicationDto>>> GetAll()
        {
            var medications = await _service.GetAllAsync();

            return Ok(medications);
        }

        // =========================================================
        // GET: api/Medication/{id}
        // User đã đăng nhập có thể xem chi tiết thuốc
        // =========================================================
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MedicationDto>> GetById(Guid id)
        {
            var medication = await _service.GetByIdAsync(id);

            if (medication == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy Medication."
                });
            }

            return Ok(medication);
        }

        // =========================================================
        // POST: api/Medication
        // Pharmacist thêm thuốc vào danh mục Medication
        // =========================================================
        [HttpPost]
        [Authorize(Roles = "Pharmacist")]
        public async Task<ActionResult<MedicationDto>> Create(
            [FromBody] CreateMedicationDto dto)
        {
            try
            {
                var medication = await _service.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = medication.Id },
                    medication);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // =========================================================
        // PUT: api/Medication/{id}
        // Pharmacist cập nhật thông tin thuốc
        // =========================================================
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Pharmacist")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateMedicationDto dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);

                if (!updated)
                {
                    return NotFound(new
                    {
                        message = "Không tìm thấy Medication."
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
    }
}