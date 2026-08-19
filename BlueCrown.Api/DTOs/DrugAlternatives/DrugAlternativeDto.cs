namespace BlueCrown.Api.DTOs.DrugAlternatives
{
    public class DrugAlternativeDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public Guid AlternativeProductId { get; set; }
        public string AlternativeProductName { get; set; } = null!;
        public string? Reason { get; set; }
        public decimal? SimilarityScore { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}