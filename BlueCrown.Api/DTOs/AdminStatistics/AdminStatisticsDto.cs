namespace BlueCrown.Api.DTOs.AdminStatistics;

public class AdminStatisticsDto
{
    public string Period { get; set; } = string.Empty;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int SalesOrderCount { get; set; }
    public decimal SalesRevenue { get; set; }
    public int InventoryReceiptCount { get; set; }
    public decimal InventoryCost { get; set; }
    public decimal Balance { get; set; }
    public List<SalesOrderStatisticDto> SalesOrders { get; set; } = new();
    public List<InventoryReceiptStatisticDto> InventoryReceipts { get; set; } = new();
}