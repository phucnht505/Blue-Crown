using BlueCrown.Api.DTOs.SymptomLogs;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface ISymptomLogService
    {
        Task<Guid?> GetPatientIdByUserIdAsync(Guid userId);
        Task<List<SymptomLogDto>> GetMyLogsAsync(Guid patientId);
        Task<SymptomLogDto?> GetByIdAsync(Guid id, Guid patientId);
        Task<SymptomLogDto?> GetLatestAsync(Guid patientId);
        Task<SymptomLogDto> CreateAsync(Guid patientId, CreateSymptomLogDto dto);
        Task<SymptomAnalysisDto> AnalyzeAsync(Guid? patientId, CreateSymptomLogDto dto);
        Task<bool> UpdateAiResultAsync(Guid id, UpdateAiResultDto dto);
    }
}