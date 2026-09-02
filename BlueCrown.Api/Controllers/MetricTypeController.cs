using BlueCrown.Api.DTOs.MetricTypes;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueCrown.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetricTypeController : ControllerBase
    {
        private readonly IMetricTypeService _service;

        public MetricTypeController(IMetricTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var metricType = await _service.GetByIdAsync(id);

            if (metricType == null)
                return NotFound(new { message = "Không tìm thấy loại chỉ số sức khỏe." });

            return Ok(metricType);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] CreateMetricTypeDto dto)
        {
            try
            {
                var metricType = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = metricType.Id }, metricType);
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

        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMetricTypeDto dto)
        {
            try
            {
                var result = await _service.UpdateAsync(id, dto);

                if (!result)
                    return NotFound(new { message = "Không tìm thấy loại chỉ số sức khỏe." });

                return Ok(new { message = "Cập nhật loại chỉ số sức khỏe thành công." });
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

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);

                if (!result)
                    return NotFound(new { message = "Không tìm thấy loại chỉ số sức khỏe." });

                return Ok(new { message = "Xóa loại chỉ số sức khỏe thành công." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}