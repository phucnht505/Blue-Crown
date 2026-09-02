using BlueCrown.Api.DTOs.AdminStatistics;

namespace BlueCrown.Api.Services.Interfaces;

public interface IAdminStatisticsService
{
    Task<AdminStatisticsDto> GetStatisticsAsync(AdminStatisticsQueryDto query);
}