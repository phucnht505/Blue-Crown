using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.HealthGoals
{
    public class UpdateHealthGoalDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Loại chỉ số sức khỏe không hợp lệ.")]
        public int MetricTypeId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá trị mục tiêu.")]
        [Range(typeof(decimal), "0.01", "999999", ErrorMessage = "Giá trị mục tiêu phải lớn hơn 0.")]
        public decimal? TargetValue { get; set; }

        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        [RegularExpression("^(active|completed|cancelled)$", ErrorMessage = "Trạng thái mục tiêu không hợp lệ.")]
        public string? Status { get; set; }
    }
}