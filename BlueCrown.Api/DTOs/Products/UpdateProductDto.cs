using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.Products
{
    public class UpdateProductDto
    {
        public Guid? MedicationId { get; set; }

        [Required(ErrorMessage = "Tên Product không được để trống.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên Product phải từ 2 đến 100 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự.")]
        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn 0.")]
        public decimal Price { get; set; }

        public bool? IsPrescriptionRequired { get; set; }

        [StringLength(100, ErrorMessage = "Hoạt chất tối đa 100 ký tự.")]
        public string? ActiveIngredient { get; set; }

        [StringLength(100, ErrorMessage = "Nhóm điều trị tối đa 100 ký tự.")]
        public string? TherapeuticGroup { get; set; }

        [StringLength(50, ErrorMessage = "Dạng bào chế tối đa 50 ký tự.")]
        public string? DosageForm { get; set; }

        [StringLength(50, ErrorMessage = "Hàm lượng tối đa 50 ký tự.")]
        public string? Strength { get; set; }

        [StringLength(500, ErrorMessage = "Đường dẫn hình ảnh tối đa 500 ký tự.")]
        public string? ImageUrl { get; set; }
    }
}