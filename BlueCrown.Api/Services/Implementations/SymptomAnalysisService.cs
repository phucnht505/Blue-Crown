using BlueCrown.Api.Services.Interfaces;
using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace BlueCrown.Api.Services.Implementations
{
    public class SymptomAnalysisService : ISymptomAnalysisService
    {
        private const double LowConfidenceThreshold = 0.20;
        private readonly HttpClient _httpClient;

        public SymptomAnalysisService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SymptomAnalysisResult> AnalyzeAsync(string symptomsDescription, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("predict", new { symptoms = symptomsDescription }, cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Dịch vụ AI hiện không thể phân tích triệu chứng.");

            var aiResult = await response.Content.ReadFromJsonAsync<AiPredictResponse>(cancellationToken: cancellationToken);

            if (aiResult == null || string.IsNullOrWhiteSpace(aiResult.PredictedDisease))
                throw new InvalidOperationException("AI không trả về kết quả hợp lệ.");

            var safety = AnalyzeSafety(symptomsDescription, aiResult.Confidence);

            return new SymptomAnalysisResult
            {
                PredictedDisease = aiResult.PredictedDisease,
                Confidence = aiResult.Confidence,
                TopPredictions = aiResult.TopPredictions.Select(x => new SymptomPredictionItem
                {
                    Disease = x.Disease,
                    Confidence = x.Confidence
                }).ToList(),
                SeverityLevel = safety.SeverityLevel,
                Advice = safety.Advice,
                ShouldSeeDoctor = safety.ShouldSeeDoctor,
                IsEmergency = safety.IsEmergency,
                IsLowConfidence = safety.IsLowConfidence
            };
        }

        private static SafetyResult AnalyzeSafety(string symptomsDescription, double confidence)
        {
            var text = NormalizeText(symptomsDescription);
            var isLowConfidence = confidence < LowConfidenceThreshold;

            var emergencyKeywords = new[]
            {
                "khong tho duoc",
                "kho tho nghiem trong",
                "dau nguc du doi",
                "mat y thuc",
                "bat tinh",
                "co giat",
                "liet nua nguoi",
                "meo mieng",
                "noi kho dot ngot",
                "ho ra mau",
                "non ra mau",
                "chay mau khong cam",
                "lo mo",
                "tim tai"
            };

            var warningKeywords = new[]
            {
                "sot cao",
                "sot keo dai",
                "dau du doi",
                "kho tho",
                "dau nguc",
                "non lien tuc",
                "non nhieu",
                "tieu chay nhieu",
                "mat nuoc",
                "chong mat nhieu",
                "phat ban toan than"
            };

            if (ContainsAny(text, emergencyKeywords))
            {
                return new SafetyResult
                {
                    SeverityLevel = "high",
                    IsEmergency = true,
                    ShouldSeeDoctor = true,
                    IsLowConfidence = isLowConfidence,
                    Advice = "Triệu chứng mô tả có dấu hiệu cảnh báo nghiêm trọng. Bạn nên tìm hỗ trợ y tế khẩn cấp hoặc đến cơ sở y tế gần nhất."
                };
            }

            if (ContainsAny(text, warningKeywords))
            {
                return new SafetyResult
                {
                    SeverityLevel = "medium",
                    IsEmergency = false,
                    ShouldSeeDoctor = true,
                    IsLowConfidence = isLowConfidence,
                    Advice = "Triệu chứng cần được theo dõi cẩn thận. Bạn nên trao đổi với bác sĩ để được đánh giá chính xác hơn."
                };
            }

            if (isLowConfidence)
            {
                return new SafetyResult
                {
                    SeverityLevel = "low",
                    IsEmergency = false,
                    ShouldSeeDoctor = true,
                    IsLowConfidence = true,
                    Advice = "AI chưa đủ độ tin cậy để xác định tình trạng từ mô tả hiện tại. Hãy mô tả triệu chứng cụ thể hơn về vị trí, mức độ, thời gian xuất hiện và các triệu chứng đi kèm. Nếu triệu chứng kéo dài hoặc nặng hơn, bạn nên trao đổi với bác sĩ."
                };
            }

            return new SafetyResult
            {
                SeverityLevel = "low",
                IsEmergency = false,
                ShouldSeeDoctor = false,
                IsLowConfidence = false,
                Advice = "Hiện chưa phát hiện dấu hiệu cảnh báo rõ ràng từ mô tả. Bạn nên tiếp tục theo dõi sức khỏe và liên hệ bác sĩ nếu triệu chứng kéo dài, xuất hiện thêm triệu chứng hoặc trở nên nghiêm trọng hơn."
            };
        }

        private static bool ContainsAny(string text, IEnumerable<string> keywords)
        {
            return keywords.Any(text.Contains);
        }

        private static string NormalizeText(string value)
        {
            var normalized = value.Replace('đ', 'd').Replace('Đ', 'D').Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var character in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                    builder.Append(character);
            }

            return builder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        }

        private sealed class SafetyResult
        {
            public string SeverityLevel { get; set; } = "low";
            public string Advice { get; set; } = null!;
            public bool ShouldSeeDoctor { get; set; }
            public bool IsEmergency { get; set; }
            public bool IsLowConfidence { get; set; }
        }

        private sealed class AiPredictResponse
        {
            [JsonPropertyName("predicted_disease")]
            public string PredictedDisease { get; set; } = null!;

            [JsonPropertyName("confidence")]
            public double Confidence { get; set; }

            [JsonPropertyName("top_predictions")]
            public List<AiPrediction> TopPredictions { get; set; } = new();
        }

        private sealed class AiPrediction
        {
            [JsonPropertyName("disease")]
            public string Disease { get; set; } = null!;

            [JsonPropertyName("confidence")]
            public double Confidence { get; set; }
        }
    }
}