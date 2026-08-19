namespace BlueCrown.Api.DTOs.HealthGoals
{
    public class CreateHealthGoalDto
    {
        public int MetricTypeId { get; set; }
        public decimal? TargetValue { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}