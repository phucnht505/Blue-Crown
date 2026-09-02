using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.HealthMetrics
{
    public class CreateHealthMetricDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Loại chỉ số sức khỏe không hợp lệ.")]
        public int MetricTypeId { get; set; }

        [Range(typeof(decimal), "0", "999999", ErrorMessage = "Giá trị chỉ số không được âm.")]
        public decimal Value { get; set; }

        public DateTime? RecordedAt { get; set; }
    }
}