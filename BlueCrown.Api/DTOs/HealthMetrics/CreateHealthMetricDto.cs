namespace BlueCrown.Api.DTOs.HealthMetrics
{
    public class CreateHealthMetricDto
    {
        public int MetricTypeId { get; set; }
        public decimal Value { get; set; }
        public DateTime? RecordedAt { get; set; }
    }
}