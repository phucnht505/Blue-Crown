namespace BlueCrown.Api.DTOs.AdminStatistics;

public class SalesOrderStatisticDto
{
    public Guid Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? GuestPhone { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string OrderStatus { get; set; } = string.Empty;
}