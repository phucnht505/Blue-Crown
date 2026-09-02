using BlueCrown.Api.DTOs.Prescriptions;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        // =========================================================
        // PATIENT
        // =========================================================

        [HttpGet("patient/my")]
        [Authorize(Roles = "patient")]
        public async Task<ActionResult<List<PrescriptionDto>>> GetPatientPrescriptions()
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var prescriptions = await _service.GetPatientPrescriptionsAsync(userId.Value);

                return Ok(prescriptions);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("patient/{id:guid}")]
        [Authorize(Roles = "patient")]
        public async Task<ActionResult<PrescriptionDto>> GetPatientPrescriptionById(Guid id)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var prescription = await _service.GetPatientPrescriptionByIdAsync(id, userId.Value);

                if (prescription == null)
                    return NotFound(new { message = "Không tìm thấy đơn thuốc." });

                return Ok(prescription);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // =========================================================
        // DOCTOR
        // =========================================================

        [HttpGet("doctor/my")]
        [Authorize(Roles = "doctor")]
        public async Task<ActionResult<List<PrescriptionDto>>> GetDoctorPrescriptions()
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var prescriptions = await _service.GetDoctorPrescriptionsAsync(userId.Value);

                return Ok(prescriptions);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("doctor/{id:guid}")]
        [Authorize(Roles = "doctor")]
        public async Task<ActionResult<PrescriptionDto>> GetDoctorPrescriptionById(Guid id)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var prescription = await _service.GetDoctorPrescriptionByIdAsync(id, userId.Value);

                if (prescription == null)
                    return NotFound(new { message = "Không tìm thấy đơn thuốc." });

                return Ok(prescription);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("doctor/medical-record/{medicalRecordId:guid}")]
        [Authorize(Roles = "doctor")]
        public async Task<ActionResult<PrescriptionDto>> GetDoctorPrescriptionByMedicalRecord(Guid medicalRecordId)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var prescription = await _service.GetDoctorPrescriptionByMedicalRecordAsync(medicalRecordId, userId.Value);

                if (prescription == null)
                    return NotFound(new { message = "Hồ sơ bệnh án này chưa có đơn thuốc." });

                return Ok(prescription);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "doctor")]
        public async Task<ActionResult<PrescriptionDto>> Create([FromBody] CreatePrescriptionDto dto)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var prescription = await _service.CreateAsync(userId.Value, dto);

                return CreatedAtAction(
                    nameof(GetDoctorPrescriptionById),
                    new { id = prescription.Id },
                    prescription);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // =========================================================
        // PHARMACIST - XEM PRESCRIPTION
        // =========================================================

        [HttpGet("pharmacist")]
        [Authorize(Roles = "pharmacist")]
        public async Task<ActionResult<List<PrescriptionDto>>> GetPharmacistPrescriptions()
        {
            var prescriptions = await _service.GetPharmacistPrescriptionsAsync();

            return Ok(prescriptions);
        }

        [HttpGet("pharmacist/{id:guid}")]
        [Authorize(Roles = "pharmacist")]
        public async Task<ActionResult<PrescriptionDto>> GetPharmacistPrescriptionById(Guid id)
        {
            var prescription = await _service.GetPharmacistPrescriptionByIdAsync(id);

            if (prescription == null)
                return NotFound(new { message = "Không tìm thấy đơn thuốc." });

            return Ok(prescription);
        }

        // =========================================================
        // PHARMACIST - DUYỆT / HỦY
        // =========================================================

        [HttpPut("pharmacist/{id:guid}/status")]
        [Authorize(Roles = "pharmacist")]
        public async Task<ActionResult<PrescriptionDto>> UpdatePharmacistStatus(
            Guid id,
            [FromBody] UpdatePrescriptionStatusDto dto)
        {
            try
            {
                var prescription = await _service.UpdatePharmacistStatusAsync(id, dto);

                if (prescription == null)
                    return NotFound(new { message = "Không tìm thấy đơn thuốc." });

                return Ok(prescription);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // =========================================================
        // PHARMACIST - CẤP THUỐC + TRỪ TỒN KHO
        // =========================================================

        [HttpPost("pharmacist/{id:guid}/dispense")]
        [Authorize(Roles = "pharmacist")]
        public async Task<ActionResult<PrescriptionDto>> Dispense(
            Guid id,
            [FromBody] DispensePrescriptionDto dto)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được Pharmacist." });

                var prescription = await _service.DispenseAsync(id, userId.Value, dto);

                if (prescription == null)
                    return NotFound(new { message = "Không tìm thấy đơn thuốc." });

                return Ok(prescription);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private Guid? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            return Guid.TryParse(claim?.Value, out var id)
                ? id
                : null;
        }
    }
}