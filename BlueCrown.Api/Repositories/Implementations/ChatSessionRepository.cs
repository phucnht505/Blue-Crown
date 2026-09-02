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
            return await BuildQuery()
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<ChatSession?> GetByIdAsync(Guid id)
        {
            return await BuildQuery().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<ChatSession>> GetByPatientIdAsync(Guid patientId)
        {
            return await BuildQuery()
                .AsNoTracking()
                .Where(x => x.PatientId == patientId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ChatSession>> GetByDoctorIdAsync(Guid doctorId)
        {
            return await BuildQuery()
                .AsNoTracking()
                .Where(x => x.DoctorId == doctorId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ChatSession>> GetUnassignedAsync()
        {
            return await BuildQuery()
                .AsNoTracking()
                .Where(x => x.DoctorId == null && x.Status == "active")
                .OrderBy(x => x.CreatedAt)
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

        private IQueryable<ChatSession> BuildQuery()
        {
            return _context.ChatSessions
                .Include(x => x.Patient)
                    .ThenInclude(x => x.User)
                .Include(x => x.Doctor)
                    .ThenInclude(x => x!.User)
                .Include(x => x.Appointments);
        }
    }
}