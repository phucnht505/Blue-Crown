using BlueCrown.Api.DTOs.Products;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueCrown.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _productService.GetAllAsync());
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
                return NotFound(new { message = "Không tìm thấy Product." });

            return Ok(product);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            try
            {
                return Ok(await _productService.SearchAsync(keyword));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("by-medication/{medicationId:guid}")]
        [Authorize(Roles = "admin,pharmacist")]
        public async Task<IActionResult> GetByMedicationId(Guid medicationId)
        {
            try
            {
                return Ok(await _productService.GetByMedicationIdAsync(medicationId));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin,pharmacist")]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            try
            {
                await _productService.CreateAsync(dto);
                return Ok(new { message = "Thêm Product thành công." });
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
        [Authorize(Roles = "admin,pharmacist")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
        {
            try
            {
                var result = await _productService.UpdateAsync(id, dto);

                if (!result)
                    return NotFound(new { message = "Không tìm thấy Product." });

                return Ok(new { message = "Cập nhật Product thành công." });
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
        [Authorize(Roles = "admin,pharmacist")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _productService.DeleteAsync(id);

                if (!result)
                    return NotFound(new { message = "Không tìm thấy Product." });

                return Ok(new { message = "Xóa Product thành công." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}