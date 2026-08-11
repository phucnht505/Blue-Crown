using BlueCrown.Api.DTOs.EcommerceOrders;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueCrown.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EcommerceOrderController : ControllerBase
    {
        private readonly IEcommerceOrderService _service;

        public EcommerceOrderController(
            IEcommerceOrderService service)
        {
            _service = service;
        }

        // =========================================================
        // GET: api/EcommerceOrder
        // Lấy tất cả đơn hàng
        // =========================================================
        [HttpGet]
        public async Task<ActionResult<List<EcommerceOrderDto>>> GetAll()
        {
            var orders = await _service.GetAllAsync();

            return Ok(orders);
        }

        // =========================================================
        // GET: api/EcommerceOrder/{id}
        // Lấy đơn hàng theo ID
        // =========================================================
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EcommerceOrderDto>> GetById(
            Guid id)
        {
            var order = await _service.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy đơn hàng."
                });
            }

            return Ok(order);
        }

        // =========================================================
        // POST: api/EcommerceOrder
        // Tạo đơn hàng
        // =========================================================
        [HttpPost]
        public async Task<ActionResult<EcommerceOrderDto>> Create(
            [FromBody] CreateEcommerceOrderDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = order.Id },
                order
            );
        }

        // =========================================================
        // DELETE: api/EcommerceOrder/{id}
        // Xóa đơn hàng
        // =========================================================
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy đơn hàng."
                });
            }

            return NoContent();
        }
    }
}