using BlueCrown.Api.DTOs.Payments;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueCrown.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // GET: api/Payment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAll()
        {
            var payments = await _paymentService.GetAllAsync();

            return Ok(payments);
        }

        // GET: api/Payment/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDto>> GetById(Guid id)
        {
            var payment = await _paymentService.GetByIdAsync(id);

            if (payment == null)
                return NotFound("Không tìm thấy payment.");

            return Ok(payment);
        }

        // GET: api/Payment/appointment/{appointmentId}
        [HttpGet("appointment/{appointmentId}")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetByAppointmentId(Guid appointmentId)
        {
            var payments = await _paymentService.GetByAppointmentIdAsync(appointmentId);

            return Ok(payments);
        }

        // POST: api/Payment
        [HttpPost]
        public async Task<ActionResult<PaymentDto>> Create(CreatePaymentDto dto)
        {
            try
            {
                var payment = await _paymentService.CreateAsync(dto);

                return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Payment/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            Guid id,
            [FromQuery] string status)
        {
            try
            {
                var result = await _paymentService.UpdateStatusAsync(id, status);

                if (!result)
                    return NotFound("Không tìm thấy payment.");

                return Ok(new
                {
                    message = "Cập nhật trạng thái payment thành công."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}