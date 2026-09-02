using BlueCrown.Api.DTOs.EcommerceOrders;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlueCrown.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EcommerceOrderController : ControllerBase
    {
        private readonly IEcommerceOrderService _service;

        public EcommerceOrderController(IEcommerceOrderService service)
        {
            _service = service;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<EcommerceOrderDto>> Create([FromBody] CreateEcommerceOrderDto dto)
        {
            try
            {
                Guid? userId = null;

                if (User.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (!Guid.TryParse(userIdClaim, out var parsedUserId))
                        return Unauthorized(new { message = "Không xác định được người dùng." });

                    userId = parsedUserId;
                }

                var order = await _service.CreateAsync(userId, dto);
                return CreatedAtAction(nameof(GetManagementById), new { id = order.Id }, order);
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

        // UC10: Guest tra cứu bằng số điện thoại, mã đơn là tùy chọn.
        [HttpPost("lookup")]
        [AllowAnonymous]
        public async Task<ActionResult<List<EcommerceOrderDto>>> LookupGuestOrders([FromBody] GuestOrderLookupDto dto)
        {
            try
            {
                var orders = await _service.LookupGuestOrdersAsync(dto);

                if (orders.Count == 0)
                    return NotFound(new { message = "Không tìm thấy đơn hàng phù hợp với thông tin đã nhập." });

                return Ok(orders);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<List<EcommerceOrderDto>>> GetMyOrders()
        {
            var userId = GetUserId();

            if (!userId.HasValue)
                return Unauthorized(new { message = "Không xác định được người dùng." });

            return Ok(await _service.GetMyOrdersAsync(userId.Value));
        }

        [HttpGet("my/{id:guid}")]
        [Authorize]
        public async Task<ActionResult<EcommerceOrderDto>> GetMyOrderById(Guid id)
        {
            var userId = GetUserId();

            if (!userId.HasValue)
                return Unauthorized(new { message = "Không xác định được người dùng." });

            var order = await _service.GetMyOrderByIdAsync(id, userId.Value);

            if (order == null)
                return NotFound(new { message = "Không tìm thấy đơn hàng." });

            return Ok(order);
        }

        [HttpPut("my/{id:guid}/cancel")]
        [Authorize]
        public async Task<ActionResult<EcommerceOrderDto>> CancelMyOrder(Guid id)
        {
            try
            {
                var userId = GetUserId();

                if (!userId.HasValue)
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var order = await _service.CancelMyOrderAsync(id, userId.Value);

                if (order == null)
                    return NotFound(new { message = "Không tìm thấy đơn hàng." });

                return Ok(order);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("manage")]
        [Authorize(Roles = "admin,pharmacist")]
        public async Task<ActionResult<List<EcommerceOrderDto>>> GetManagementOrders()
        {
            return Ok(await _service.GetManagementOrdersAsync());
        }

        [HttpGet("manage/{id:guid}")]
        [Authorize(Roles = "admin,pharmacist")]
        public async Task<ActionResult<EcommerceOrderDto>> GetManagementById(Guid id)
        {
            var order = await _service.GetManagementOrderByIdAsync(id);

            if (order == null)
                return NotFound(new { message = "Không tìm thấy đơn hàng." });

            return Ok(order);
        }

        [HttpPut("manage/{id:guid}/status")]
        [Authorize(Roles = "admin,pharmacist")]
        public async Task<ActionResult<EcommerceOrderDto>> UpdateStatus(Guid id, [FromBody] UpdateEcommerceOrderStatusDto dto)
        {
            try
            {
                var order = await _service.UpdateStatusAsync(id, dto);

                if (order == null)
                    return NotFound(new { message = "Không tìm thấy đơn hàng." });

                return Ok(order);
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

        private Guid? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(claim, out var userId) ? userId : null;
        }
    }
}