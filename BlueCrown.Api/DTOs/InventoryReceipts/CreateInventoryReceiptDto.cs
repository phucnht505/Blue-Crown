using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.InventoryReceipts
{
    public class CreateInventoryReceiptDto
    {
        [Required(ErrorMessage = "Vui lòng chọn nhà cung cấp.")]
        public Guid? SupplierId { get; set; }

        [Required(ErrorMessage = "Phiếu nhập phải có danh sách sản phẩm.")]
        [MinLength(1, ErrorMessage = "Phiếu nhập phải có ít nhất một sản phẩm.")]
        public List<CreateReceiptDetailDto> Details { get; set; } = new();
    }
}