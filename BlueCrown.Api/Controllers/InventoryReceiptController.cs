using BlueCrown.Api.DTOs.InventoryReceipts;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlueCrown.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InventoryReceiptController : ControllerBase
    {
        private readonly IInventoryReceiptService _receiptService;

        public InventoryReceiptController(IInventoryReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        [HttpGet]
        [Authorize(Roles = "admin,pharmacist")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _receiptService.GetAllAsync());
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "admin,pharmacist")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var receipt = await _receiptService.GetByIdAsync(id);

            if (receipt == null)
                return NotFound(new { message = "Không tìm thấy phiếu nhập." });

            return Ok(receipt);
        }

        [HttpPost]
        [Authorize(Roles = "pharmacist")]
        public async Task<IActionResult> Create([FromBody] CreateInventoryReceiptDto dto)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized(new { message = "Không xác định được Pharmacist." });

                await _receiptService.CreateAsync(dto, userId.Value);

                return Ok(new { message = "Tạo phiếu nhập thành công." });
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

        [HttpPut("{id:guid}/approve")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Approve(Guid id)
        {
            try
            {
                var adminId = GetUserId();

                if (adminId == null)
                    return Unauthorized(new { message = "Không xác định được quản trị viên." });

                var result = await _receiptService.ApproveAsync(id, adminId.Value);

                if (!result)
                    return NotFound(new { message = "Không tìm thấy phiếu nhập." });

                return Ok(new { message = "Duyệt phiếu nhập thành công." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:guid}/reject")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Reject(Guid id)
        {
            try
            {
                var adminId = GetUserId();

                if (adminId == null)
                    return Unauthorized(new { message = "Không xác định được quản trị viên." });

                var result = await _receiptService.RejectAsync(id, adminId.Value);

                if (!result)
                    return NotFound(new { message = "Không tìm thấy phiếu nhập." });

                return Ok(new { message = "Đã từ chối phiếu nhập." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private Guid? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim?.Value, out var id) ? id : null;
        }
    }
}