using BlueCrown.Api.DTOs.MedicalRecords;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        // GET: api/MedicalRecord
        [HttpGet]
        public async Task<ActionResult<List<MedicalRecordDto>>> GetAll()
        {
            var records = await _service.GetAllAsync();

            return Ok(records);
        }

        // GET: api/MedicalRecord/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MedicalRecordDto>> GetById(Guid id)
        {
            var record = await _service.GetByIdAsync(id);

            if (record == null)
                return NotFound();

            return Ok(record);
        }

        // GET: api/MedicalRecord/patient/{patientId}
        [HttpGet("patient/{patientId:guid}")]
        public async Task<ActionResult<List<MedicalRecordDto>>> GetByPatientId(Guid patientId)
        {
            var records = await _service.GetByPatientIdAsync(patientId);

            return Ok(records);
        }

        // GET: api/MedicalRecord/doctor/{doctorId}
        [HttpGet("doctor/{doctorId:guid}")]
        public async Task<ActionResult<List<MedicalRecordDto>>> GetByDoctorId(Guid doctorId)
        {
            var records = await _service.GetByDoctorIdAsync(doctorId);

            return Ok(records);
        }

        // GET: api/MedicalRecord/appointment/{appointmentId}
        [HttpGet("appointment/{appointmentId:guid}")]
        public async Task<ActionResult<MedicalRecordDto>> GetByAppointmentId(Guid appointmentId)
        {
            var record = await _service.GetByAppointmentIdAsync(appointmentId);

            if (record == null)
                return NotFound();

            return Ok(record);
        }

        // POST: api/MedicalRecord
        [HttpPost]
        public async Task<ActionResult<MedicalRecordDto>> Create(
            [FromBody] CreateMedicalRecordDto dto)
        {
            var record = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = record.Id },
                record);
        }

        // PUT: api/MedicalRecord/{id}
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<MedicalRecordDto>> Update(
            Guid id,
            [FromBody] CreateMedicalRecordDto dto)
        {
            var record = await _service.UpdateAsync(id, dto);

            if (record == null)
                return NotFound();

            return Ok(record);
        }

        // DELETE: api/MedicalRecord/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}