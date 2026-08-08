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

        public InventoryReceiptController(
            IInventoryReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Pharmacist")]
        public async Task<IActionResult> GetAll()
        {
            var receipts = await _receiptService.GetAllAsync();

            return Ok(receipts);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin,Pharmacist")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var receipt = await _receiptService.GetByIdAsync(id);

            if (receipt == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy phiếu nhập."
                });
            }

            return Ok(receipt);
        }

        [HttpPost]
        [Authorize(Roles = "Pharmacist")]
        public async Task<IActionResult> Create(
            [FromBody] CreateInventoryReceiptDto dto)
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized(new
                {
                    message = "Không xác định được người dùng."
                });
            }

            await _receiptService.CreateAsync(dto, userId);

            return Ok(new
            {
                message = "Tạo phiếu nhập thành công."
            });
        }

        [HttpPut("{id:guid}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var adminIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(adminIdClaim, out Guid adminId))
            {
                return Unauthorized(new
                {
                    message = "Không xác định được quản trị viên."
                });
            }

            var result =
                await _receiptService.ApproveAsync(id, adminId);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy phiếu nhập."
                });
            }

            return Ok(new
            {
                message = "Duyệt phiếu nhập thành công."
            });
        }
    }
}