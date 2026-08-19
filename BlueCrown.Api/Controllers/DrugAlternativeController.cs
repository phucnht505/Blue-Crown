using BlueCrown.Api.DTOs.DrugAlternatives;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueCrown.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DrugAlternativeController : ControllerBase
    {
        private readonly IDrugAlternativeService _service;

        public DrugAlternativeController(IDrugAlternativeService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var alternative = await _service.GetByIdAsync(id);

            if (alternative == null)
                return NotFound(new { message = "Không tìm thấy thuốc thay thế." });

            return Ok(alternative);
        }

        [HttpGet("product/{productId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetByProductId(Guid productId)
        {
            try
            {
                return Ok(await _service.GetByProductIdAsync(productId));
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateDrugAlternativeDto dto)
        {
            try
            {
                var alternative = await _service.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = alternative.Id },
                    alternative);
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

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, UpdateDrugAlternativeDto dto)
        {
            try
            {
                var result = await _service.UpdateAsync(id, dto);

                if (!result)
                    return NotFound(new { message = "Không tìm thấy thuốc thay thế." });

                return Ok(new { message = "Cập nhật thuốc thay thế thành công." });
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

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
                return NotFound(new { message = "Không tìm thấy thuốc thay thế." });

            return Ok(new { message = "Xóa thuốc thay thế thành công." });
        }
    }
}