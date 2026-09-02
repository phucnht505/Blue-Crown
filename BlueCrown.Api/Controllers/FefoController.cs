using BlueCrown.Api.DTOs.InventoryReceipts;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueCrown.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "pharmacist,admin")]
    public class FefoController : ControllerBase
    {
        private readonly IFefoService _fefoService;

        public FefoController(IFefoService fefoService)
        {
            _fefoService = fefoService;
        }

        [HttpGet("{productId:guid}")]
        public async Task<ActionResult<List<ReceiptDetailDto>>> GetFefo(Guid productId)
        {
            var result = await _fefoService.GetFefoAsync(productId);
            return Ok(result);
        }
    }
}