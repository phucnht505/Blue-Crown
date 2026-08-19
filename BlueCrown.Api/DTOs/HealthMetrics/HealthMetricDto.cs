namespace BlueCrown.Api.DTOs.HealthMetrics
{
    public class HealthMetricDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public int MetricTypeId { get; set; }
        public string MetricTypeCode { get; set; } = null!;
        public string MetricTypeName { get; set; } = null!;
        public string MetricTypeUnit { get; set; } = null!;
        public decimal Value { get; set; }
        public DateTime RecordedAt { get; set; }
        public decimal? NormalMin { get; set; }
        public decimal? NormalMax { get; set; }
    }
}