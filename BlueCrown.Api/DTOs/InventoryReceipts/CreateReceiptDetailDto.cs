using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.InventoryReceipts
{
    public class CreateReceiptDetailDto
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Số lô không được để trống.")]
        [StringLength(100, ErrorMessage = "Số lô tối đa 100 ký tự.")]
        public string BatchNumber { get; set; } = string.Empty;

        [Required]
        public DateOnly ExpirationDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng nhập phải lớn hơn 0.")]
        public int QuantityImported { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá nhập phải lớn hơn 0.")]
        public decimal ImportPrice { get; set; }
    }
}