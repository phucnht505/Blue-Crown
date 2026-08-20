namespace BlueCrown.Api.DTOs.MetricTypes
{
    public class MetricTypeDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Unit { get; set; } = null!;
        public decimal? NormalMin { get; set; }
        public decimal? NormalMax { get; set; }
    }
}