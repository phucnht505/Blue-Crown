using System.ComponentModel.DataAnnotations;
namespace BlueCrown.Api.DTOs.Products
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Tên thuốc không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên thuốc tối đa 100 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá thuốc không được để trống.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá thuốc phải lớn hơn 0.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn không hợp lệ.")]
        public int? StockQuantity { get; set; }

        public bool? IsPrescriptionRequired { get; set; }

        [StringLength(100)]
        public string? ActiveIngredient { get; set; }

        [StringLength(100)]
        public string? TherapeuticGroup { get; set; }

        [StringLength(50)]
        public string? DosageForm { get; set; }

        [StringLength(50)]
        public string? Strength { get; set; }
    }
}
