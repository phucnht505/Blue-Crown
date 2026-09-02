namespace BlueCrown.Api.DTOs.AdminStatistics;

public class InventoryReceiptStatisticDto
{
    public Guid Id { get; set; }
    public DateTime? ReceiptDate { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public decimal TotalCost { get; set; }
    public string Status { get; set; } = string.Empty;
}