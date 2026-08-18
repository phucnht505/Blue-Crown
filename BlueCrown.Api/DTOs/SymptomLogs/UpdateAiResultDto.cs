namespace BlueCrown.Api.DTOs.SymptomLogs
{
    public class UpdateAiResultDto
    {
        public string? PredictedDisease { get; set; }

        public string? SeverityLevel { get; set; }

        public string? AiAdvice { get; set; }
    }
}