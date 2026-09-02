using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.Prescriptions
{
    public class DispensePrescriptionItemDto
    {
        public Guid PrescriptionItemId { get; set; }

        public Guid ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng cấp phải lớn hơn 0.")]
        public int QuantityDispensed { get; set; }
    }
}