namespace BlueCrown.Api.DTOs.InventoryReceipts
{
    public class InventoryReceiptDto
    {
        public Guid Id { get; set; }
        public Guid? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public Guid? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public Guid? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; }
        public decimal? TotalCost { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string? Status { get; set; }
        public List<ReceiptDetailDto> Details { get; set; } = new();
    }

    public class ReceiptDetailDto
    {
        public Guid Id { get; set; }
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? StockQuantity { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public DateOnly ExpirationDate { get; set; }
        public int QuantityImported { get; set; }
        public decimal ImportPrice { get; set; }
    }
}