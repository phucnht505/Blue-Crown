using BlueCrown.Api.DTOs.ChatSessions;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IChatSessionService
    {
        Task<List<ChatSessionDto>> GetMySessionsAsync(Guid patientId);
        Task<List<ChatSessionDto>> GetDoctorSessionsAsync(Guid doctorId);
        Task<List<ChatSessionDto>> GetAvailableSessionsAsync();
        Task<ChatSessionDto?> GetByIdAsync(Guid id, Guid userId);
        Task<ChatSessionDto> CreateAsync(Guid patientId, CreateChatSessionDto dto);
        Task<bool> AssignDoctorAsync(Guid id, Guid doctorId);
        Task<bool> UpdateStatusAsync(Guid id, Guid userId, UpdateChatSessionStatusDto dto);
        Task<Guid?> GetPatientIdByUserIdAsync(Guid userId);
        Task<Guid?> GetDoctorIdByUserIdAsync(Guid userId);
    }
}