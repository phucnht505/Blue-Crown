using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class ChatSessionRepository : IChatSessionRepository
    {
        private readonly BlueCrownContext _context;

        public ChatSessionRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task<List<ChatSession>> GetAllAsync()
        {
            return await _context.ChatSessions
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ChatSession?> GetByIdAsync(Guid id)
        {
            return await _context.ChatSessions
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<ChatSession>> GetByPatientIdAsync(
            Guid patientId)
        {
            return await _context.ChatSessions
                .AsNoTracking()
                .Where(x => x.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<List<ChatSession>> GetByDoctorIdAsync(
            Guid doctorId)
        {
            return await _context.ChatSessions
                .AsNoTracking()
                .Where(x => x.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task AddAsync(ChatSession session)
        {
            await _context.ChatSessions.AddAsync(session);
        }

        public async Task UpdateAsync(ChatSession session)
        {
            _context.ChatSessions.Update(session);

            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}