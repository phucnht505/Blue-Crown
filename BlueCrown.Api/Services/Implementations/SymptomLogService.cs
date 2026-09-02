using BlueCrown.Api.DTOs.SymptomLogs;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Services.Implementations
{
    public class SymptomLogService : ISymptomLogService
    {
        private readonly ISymptomLogRepository _repository;
        private readonly IAutoPrescriptionRepository _autoPrescriptionRepository;
        private readonly ISymptomAnalysisService _symptomAnalysisService;
        private readonly BlueCrownContext _context;

        public SymptomLogService(ISymptomLogRepository repository, IAutoPrescriptionRepository autoPrescriptionRepository, ISymptomAnalysisService symptomAnalysisService, BlueCrownContext context)
        {
            _repository = repository;
            _autoPrescriptionRepository = autoPrescriptionRepository;
            _symptomAnalysisService = symptomAnalysisService;
            _context = context;
        }

        public async Task<Guid?> GetPatientIdByUserIdAsync(Guid userId)
        {
            return await _context.PatientProfiles
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .Select(p => (Guid?)p.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<SymptomLogDto>> GetMyLogsAsync(Guid patientId)
        {
            var logs = await _repository.GetByPatientIdAsync(patientId);
            return logs.Select(MapToDto).ToList();
        }

        public async Task<SymptomLogDto?> GetByIdAsync(Guid id, Guid patientId)
        {
            var log = await _repository.GetByIdAsync(id);

            if (log == null)
                return null;

            if (log.PatientId != patientId)
                throw new UnauthorizedAccessException("Bạn không có quyền xem Symptom Log này.");

            return MapToDto(log);
        }

        public async Task<SymptomLogDto?> GetLatestAsync(Guid patientId)
        {
            var log = await _repository.GetLatestByPatientIdAsync(patientId);
            return log == null ? null : MapToDto(log);
        }

        public async Task<SymptomLogDto> CreateAsync(Guid patientId, CreateSymptomLogDto dto)
        {
            var description = ValidateSymptoms(dto.SymptomsDescription);

            var symptomLog = new SymptomLog
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                SymptomsDescription = description,
                PredictedDisease = null,
                SeverityLevel = null,
                AiAdvice = null,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(symptomLog);
            await _repository.SaveChangesAsync();

            return MapToDto(symptomLog);
        }

        public async Task<SymptomAnalysisDto> AnalyzeAsync(Guid? patientId, CreateSymptomLogDto dto)
        {
            var description = ValidateSymptoms(dto.SymptomsDescription);
            var analysis = await _symptomAnalysisService.AnalyzeAsync(description);

            SymptomLogDto? symptomLogDto = null;

            if (patientId.HasValue)
            {
                var symptomLog = new SymptomLog
                {
                    Id = Guid.NewGuid(),
                    PatientId = patientId.Value,
                    SymptomsDescription = description,
                    PredictedDisease = analysis.PredictedDisease,
                    SeverityLevel = analysis.SeverityLevel,
                    AiAdvice = analysis.Advice,
                    CreatedAt = DateTime.UtcNow
                };

                await _repository.AddAsync(symptomLog);
                await _repository.SaveChangesAsync();

                symptomLogDto = MapToDto(symptomLog);
            }

            AutoPrescription? recommendation = null;

            // BR-AI-004: Chỉ trường hợp cảnh báo thấp, AI đủ tin cậy và không cần bác sĩ mới xét gợi ý Product.
            if (analysis.SeverityLevel == "low" && !analysis.ShouldSeeDoctor && !analysis.IsLowConfidence && analysis.Confidence >= 0.45)
                recommendation = await _autoPrescriptionRepository.GetByDiseaseNameAsync(analysis.PredictedDisease);

            // BR-AI-005: AI không tự gợi ý Product bắt buộc kê đơn.
            if (recommendation?.RecommendedProduct?.IsPrescriptionRequired == true)
                recommendation = null;

            return new SymptomAnalysisDto
            {
                SymptomLog = symptomLogDto,
                PredictedDisease = analysis.PredictedDisease,
                Confidence = analysis.Confidence,
                TopPredictions = analysis.TopPredictions.Select(x => new DiseasePredictionDto
                {
                    Disease = x.Disease,
                    Confidence = x.Confidence
                }).ToList(),
                SeverityLevel = analysis.SeverityLevel,
                Advice = analysis.Advice,
                IsLowConfidence = analysis.IsLowConfidence,
                RecommendedProductId = recommendation?.RecommendedProductId,
                RecommendedProductName = recommendation?.RecommendedProduct?.Name,
                DosageInstructions = recommendation?.DosageInstructions,
                ShouldSeeDoctor = analysis.ShouldSeeDoctor,
                IsEmergency = analysis.IsEmergency
            };
        }

        public async Task<bool> UpdateAiResultAsync(Guid id, UpdateAiResultDto dto)
        {
            var log = await _repository.GetByIdAsync(id);

            if (log == null)
                return false;

            log.PredictedDisease = string.IsNullOrWhiteSpace(dto.PredictedDisease) ? null : dto.PredictedDisease.Trim();
            log.SeverityLevel = string.IsNullOrWhiteSpace(dto.SeverityLevel) ? null : dto.SeverityLevel.Trim();
            log.AiAdvice = string.IsNullOrWhiteSpace(dto.AiAdvice) ? null : dto.AiAdvice.Trim();

            await _repository.UpdateAsync(log);
            await _repository.SaveChangesAsync();

            return true;
        }

        private static string ValidateSymptoms(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("Vui lòng nhập triệu chứng.");

            var description = value.Trim();

            if (description.Length < 5)
                throw new Exception("Mô tả triệu chứng phải có ít nhất 5 ký tự.");

            if (description.Length > 2000)
                throw new Exception("Mô tả triệu chứng không được vượt quá 2000 ký tự.");

            if (!description.Any(char.IsLetter))
                throw new Exception("Mô tả triệu chứng phải chứa nội dung chữ hợp lệ.");

            return description;
        }

        private static SymptomLogDto MapToDto(SymptomLog log)
        {
            return new SymptomLogDto
            {
                Id = log.Id,
                PatientId = log.PatientId,
                SymptomsDescription = log.SymptomsDescription,
                PredictedDisease = log.PredictedDisease,
                SeverityLevel = log.SeverityLevel,
                AiAdvice = log.AiAdvice,
                CreatedAt = log.CreatedAt
            };
        }
    }
}