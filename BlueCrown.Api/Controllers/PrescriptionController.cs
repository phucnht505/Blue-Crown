using BlueCrown.Api.DTOs.Prescriptions;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueCrown.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionService _service;

        public PrescriptionController(IPrescriptionService service)
        {
            _service = service;
        }

        // GET: api/Prescription
        [HttpGet]
        public async Task<ActionResult<List<PrescriptionDto>>> GetAll()
        {
            var prescriptions = await _service.GetAllAsync();

            return Ok(prescriptions);
        }

        // GET: api/Prescription/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PrescriptionDto>> GetById(Guid id)
        {
            var prescription = await _service.GetByIdAsync(id);

            if (prescription == null)
                return NotFound(new
                {
                    message = "Prescription not found."
                });

            return Ok(prescription);
        }

        // POST: api/Prescription
        [HttpPost]
        public async Task<ActionResult<PrescriptionDto>> Create(
            [FromBody] CreatePrescriptionDto dto)
        {
            var prescription = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = prescription.Id },
                prescription);
        }

        // PUT: api/Prescription/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdatePrescriptionDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);

            if (!updated)
                return NotFound(new
                {
                    message = "Prescription not found."
                });

            return NoContent();
        }

        // DELETE: api/Prescription/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound(new
                {
                    message = "Prescription not found."
                });

            return NoContent();
        }
    }
}