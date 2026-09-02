using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.Medications
{
    public class UpdateMedicationDto
    {
        [Required(ErrorMessage = "Tên Medication không được để trống.")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Tên Medication phải từ 2 đến 150 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(150, ErrorMessage = "Tên generic tối đa 150 ký tự.")]
        public string? GenericName { get; set; }

        [StringLength(100, ErrorMessage = "Nhóm thuốc tối đa 100 ký tự.")]
        public string? Category { get; set; }
    }
}