namespace BlueCrown.Api.DTOs.AdminStatistics;

public class AdminStatisticsQueryDto
{
    public string Period { get; set; } = "day";
    public DateTime? Date { get; set; }
    public int? Month { get; set; }
    public int? Year { get; set; }
}