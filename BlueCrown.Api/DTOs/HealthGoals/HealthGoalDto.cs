namespace BlueCrown.Api.DTOs.HealthGoals
{
    public class HealthGoalDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public int MetricTypeId { get; set; }
        public string MetricTypeCode { get; set; } = null!;
        public string MetricTypeName { get; set; } = null!;
        public string MetricTypeUnit { get; set; } = null!;
        public decimal? TargetValue { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Status { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string CreatedByRole { get; set; } = string.Empty;
    }
}