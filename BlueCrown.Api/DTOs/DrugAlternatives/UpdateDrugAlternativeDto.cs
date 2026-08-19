namespace BlueCrown.Api.DTOs.DrugAlternatives
{
    public class UpdateDrugAlternativeDto
    {
        public Guid ProductId { get; set; }
        public Guid AlternativeProductId { get; set; }
        public string? Reason { get; set; }
        public decimal? SimilarityScore { get; set; }
    }
}