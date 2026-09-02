namespace BlueCrown.Api.DTOs.SymptomLogs
{
    public class DiseasePredictionDto
    {
        public string Disease { get; set; } = null!;
        public double Confidence { get; set; }
    }

    public class SymptomAnalysisDto
    {
        public SymptomLogDto? SymptomLog { get; set; }
        public string PredictedDisease { get; set; } = null!;
        public double Confidence { get; set; }
        public List<DiseasePredictionDto> TopPredictions { get; set; } = new();
        public string SeverityLevel { get; set; } = "low";
        public string Advice { get; set; } = null!;
        public bool IsLowConfidence { get; set; }
        public Guid? RecommendedProductId { get; set; }
        public string? RecommendedProductName { get; set; }
        public string? DosageInstructions { get; set; }
        public bool ShouldSeeDoctor { get; set; }
        public bool IsEmergency { get; set; }
    }
}