using BlueCrown.Api.DTOs.AdminStatistics;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueCrown.Api.Controllers;

[ApiController]
[Route("api/admin-statistics")]
[Authorize(Roles = "admin")]
public class AdminStatisticsController : ControllerBase
{
    private readonly IAdminStatisticsService _service;

    public AdminStatisticsController(IAdminStatisticsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<AdminStatisticsDto>> GetStatistics([FromQuery] AdminStatisticsQueryDto query)
    {
        try
        {
            return Ok(await _service.GetStatisticsAsync(query));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}