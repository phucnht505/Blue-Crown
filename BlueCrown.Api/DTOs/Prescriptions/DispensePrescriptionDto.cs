using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.Prescriptions
{
    public class DispensePrescriptionDto
    {
        [Required(ErrorMessage = "Vui lòng nhập danh sách thuốc được cấp.")]
        [MinLength(1, ErrorMessage = "Phải có ít nhất một thuốc được cấp.")]
        public List<DispensePrescriptionItemDto> Items { get; set; } = new();
    }
}