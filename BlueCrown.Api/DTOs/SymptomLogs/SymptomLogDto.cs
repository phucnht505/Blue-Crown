using System;

namespace BlueCrown.Api.DTOs.SymptomLogs
{
    public class SymptomLogDto
    {
        public Guid Id { get; set; }

        public Guid? PatientId { get; set; }

        public string SymptomsDescription { get; set; } = null!;

        public string? PredictedDisease { get; set; }

        public string? SeverityLevel { get; set; }

        public string? AiAdvice { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}