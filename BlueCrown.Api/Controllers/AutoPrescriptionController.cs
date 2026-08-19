using BlueCrown.Api.DTOs.AutoPrescriptions;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueCrown.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AutoPrescriptionController : ControllerBase
    {
        private readonly IAutoPrescriptionService _service;

        public AutoPrescriptionController(IAutoPrescriptionService service)
        {
            _service = service;
        }

        // GET: api/AutoPrescription
        // Admin xem các cấu hình AutoPrescription
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var prescriptions = await _service.GetAllAsync();
            return Ok(prescriptions);
        }

        // GET: api/AutoPrescription/{id}
        // Admin xem chi tiết cấu hình
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var prescription = await _service.GetByIdAsync(id);

            if (prescription == null)
                return NotFound(new { message = "Không tìm thấy đơn thuốc tự động." });

            return Ok(prescription);
        }

        // GET: api/AutoPrescription/disease/{diseaseName}
        // Admin tra cứu cấu hình theo bệnh
        [HttpGet("disease/{diseaseName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByDiseaseName(string diseaseName)
        {
            if (string.IsNullOrWhiteSpace(diseaseName))
                return BadRequest(new { message = "Tên bệnh không được để trống." });

            var prescription = await _service.GetByDiseaseNameAsync(diseaseName);

            if (prescription == null)
                return NotFound(new { message = "Không tìm thấy đơn thuốc cho bệnh này." });

            return Ok(prescription);
        }

        // POST: api/AutoPrescription
        // Admin tạo cấu hình AutoPrescription
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateAutoPrescriptionDto dto)
        {
            try
            {
                var prescription = await _service.AddAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = prescription.Id },
                    prescription);
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

        // PUT: api/AutoPrescription/{id}
        // Admin cập nhật cấu hình AutoPrescription
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAutoPrescriptionDto dto)
        {
            try
            {
                var result = await _service.UpdateAsync(id, dto);

                if (!result)
                    return NotFound(new { message = "Không tìm thấy đơn thuốc tự động." });

                return NoContent();
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

        // DELETE: api/AutoPrescription/{id}
        // Admin xóa cấu hình AutoPrescription
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
                return NotFound(new { message = "Không tìm thấy đơn thuốc tự động." });

            return NoContent();
        }
    }
}