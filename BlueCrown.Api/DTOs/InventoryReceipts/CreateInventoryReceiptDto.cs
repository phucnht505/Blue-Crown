using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.InventoryReceipts
{
    public class CreateInventoryReceiptDto
    {
        public Guid? SupplierId { get; set; }

        [Required]
        [MinLength(1)]
        public List<CreateReceiptDetailDto> Details { get; set; } = new();
    }
}