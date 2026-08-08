using BlueCrown.Api.DTOs.Checkout;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlueCrown.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpPost]
        public async Task<ActionResult<CheckoutResponseDto>> Create([FromBody] CreateCheckoutDto dto)
        {
            try
            {
                Guid? userId = null;

                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrWhiteSpace(userIdClaim) && Guid.TryParse(userIdClaim, out Guid parsedUserId))
                {
                    userId = parsedUserId;
                }

                var result = await _checkoutService.CreateAsync(dto, userId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CheckoutResponseDto>> GetById(Guid id)
        {
            var result = await _checkoutService.GetByIdAsync(id);

            if (result == null)
                return NotFound(new
                {
                    message = "Không tìm thấy đơn hàng."
                });

            return Ok(result);
        }
    }
}