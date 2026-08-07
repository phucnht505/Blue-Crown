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
            var products = await _productService.GetAllAsync();

            return Ok(products);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy thuốc."
                });
            }

            return Ok(product);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new
                {
                    message = "Vui lòng nhập từ khóa tìm kiếm."
                });
            }

            var products = await _productService.SearchAsync(keyword);

            return Ok(products);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Pharmacist")]
        public async Task<IActionResult> Create(
            [FromBody] CreateProductDto dto)
        {
            await _productService.CreateAsync(dto);

            return Ok(new
            {
                message = "Thêm thuốc thành công."
            });
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,Pharmacist")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateProductDto dto)
        {
            var result = await _productService.UpdateAsync(id, dto);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy thuốc."
                });
            }

            return Ok(new
            {
                message = "Cập nhật thuốc thành công."
            });
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin,Pharmacist")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _productService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy thuốc."
                });
            }

            return Ok(new
            {
                message = "Xóa thuốc thành công."
            });
        }
    }
}