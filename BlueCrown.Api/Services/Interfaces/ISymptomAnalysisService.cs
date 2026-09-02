namespace BlueCrown.Api.Services.Interfaces
{
    public class SymptomPredictionItem
    {
        public string Disease { get; set; } = null!;
        public double Confidence { get; set; }
    }

    public class SymptomAnalysisResult
    {
        public string PredictedDisease { get; set; } = null!;
        public double Confidence { get; set; }
        public List<SymptomPredictionItem> TopPredictions { get; set; } = new();
        public string SeverityLevel { get; set; } = "low";
        public string Advice { get; set; } = null!;
        public bool ShouldSeeDoctor { get; set; }
        public bool IsEmergency { get; set; }
        public bool IsLowConfidence { get; set; }
    }

    public interface ISymptomAnalysisService
    {
        Task<SymptomAnalysisResult> AnalyzeAsync(string symptomsDescription, CancellationToken cancellationToken = default);
    }
}