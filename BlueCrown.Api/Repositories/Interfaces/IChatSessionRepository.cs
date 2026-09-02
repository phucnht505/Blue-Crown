using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IChatSessionRepository
    {
        Task<List<ChatSession>> GetAllAsync();
        Task<ChatSession?> GetByIdAsync(Guid id);
        Task<List<ChatSession>> GetByPatientIdAsync(Guid patientId);
        Task<List<ChatSession>> GetByDoctorIdAsync(Guid doctorId);
        Task<List<ChatSession>> GetUnassignedAsync();
        Task AddAsync(ChatSession session);
        Task UpdateAsync(ChatSession session);
        Task SaveChangesAsync();
    }
}