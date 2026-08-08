using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.InventoryReceipts
{
    public class CreateReceiptDetailDto
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public string BatchNumber { get; set; } = string.Empty;

        [Required]
        public DateOnly ExpirationDate { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int QuantityImported { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal ImportPrice { get; set; }
    }
}