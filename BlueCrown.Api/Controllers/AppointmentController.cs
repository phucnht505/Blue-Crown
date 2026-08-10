using BlueCrown.Api.DTOs.Appointments;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueCrown.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(
            IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        // GET: api/Appointment
        [HttpGet]
        public async Task<ActionResult<List<AppointmentDto>>> GetAll()
        {
            var appointments = await _appointmentService.GetAllAsync();

            return Ok(appointments);
        }

        // GET: api/Appointment/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AppointmentDto>> GetById(Guid id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);

            if (appointment == null)
            {
                return NotFound(new
                {
                    message = "Appointment not found."
                });
            }

            return Ok(appointment);
        }

        // GET: api/Appointment/patient/{patientId}
        [HttpGet("patient/{patientId:guid}")]
        public async Task<ActionResult<List<AppointmentDto>>> GetByPatientId(
            Guid patientId)
        {
            var appointments = await _appointmentService.GetByPatientIdAsync(patientId);

            return Ok(appointments);
        }

        // POST: api/Appointment
        [HttpPost]
        public async Task<ActionResult<AppointmentDto>> Create(
            [FromBody] CreateAppointmentDto dto)
        {
            var appointment =
                await _appointmentService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = appointment.Id },
                appointment
            );
        }

        // DELETE: api/Appointment/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted =
                await _appointmentService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Appointment not found."
                });
            }

            return NoContent();
        }
    }
}