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
        private readonly BlueCrownContext _context;

        public SymptomLogService(ISymptomLogRepository repository, BlueCrownContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<Guid?> GetPatientIdByUserIdAsync(Guid userId)
        {
            var patient = await _context.PatientProfiles.AsNoTracking().FirstOrDefaultAsync(p => p.UserId == userId);
            return patient?.Id;
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
                throw new UnauthorizedAccessException("You do not have access to this symptom log.");

            return MapToDto(log);
        }

        public async Task<SymptomLogDto?> GetLatestAsync(Guid patientId)
        {
            var log = await _repository.GetLatestByPatientIdAsync(patientId);

            if (log == null)
                return null;

            return MapToDto(log);
        }

        public async Task<SymptomLogDto> CreateAsync(Guid patientId, CreateSymptomLogDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SymptomsDescription))
                throw new Exception("Symptoms description cannot be empty.");

            var symptomLog = new SymptomLog
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                SymptomsDescription = dto.SymptomsDescription.Trim(),
                PredictedDisease = null,
                SeverityLevel = null,
                AiAdvice = null,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(symptomLog);
            await _repository.SaveChangesAsync();

            var createdLog = await _repository.GetByIdAsync(symptomLog.Id);

            if (createdLog == null)
                throw new Exception("Failed to retrieve created symptom log.");

            return MapToDto(createdLog);
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