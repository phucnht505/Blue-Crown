using BlueCrown.Api.DTOs.ChatSessions;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Services.Implementations
{
    public class ChatSessionService : IChatSessionService
    {
        private readonly IChatSessionRepository _repository;
        private readonly BlueCrownContext _context;

        public ChatSessionService(IChatSessionRepository repository, BlueCrownContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<List<ChatSessionDto>> GetMySessionsAsync(Guid patientId)
        {
            var sessions =
                await _repository.GetByPatientIdAsync(patientId);

            return sessions
                .Select(MapToDto)
                .ToList();
        }

        public async Task<List<ChatSessionDto>> GetDoctorSessionsAsync(Guid doctorId)
        {
            var sessions = await _repository.GetByDoctorIdAsync(doctorId);

            return sessions.Select(MapToDto).ToList();
        }

        public async Task<ChatSessionDto?> GetByIdAsync(Guid id)
        {
            var session = await _repository.GetByIdAsync(id);

            if (session == null)
            {
                return null;
            }

            return MapToDto(session);
        }

        public async Task<ChatSessionDto> CreateAsync(Guid patientId, CreateChatSessionDto dto)
        {
            // Kiểm tra PatientProfile
            var patientExists = await _context.PatientProfiles
                .AnyAsync(p => p.Id == patientId);

            if (!patientExists)
            {
                throw new Exception("Patient profile not found.");
            }

            // Nếu có AI Symptom Log thì kiểm tra tồn tại
            if (dto.AiSymptomLogId.HasValue)
            {
                var symptomLogExists = await _context.SymptomLogs
                    .AnyAsync(s => s.Id == dto.AiSymptomLogId.Value);

                if (!symptomLogExists)
                {
                    throw new Exception("AI symptom log not found.");
                }
            }

            var session = new ChatSession
            {
                Id = Guid.NewGuid(),

                PatientId = patientId,

                DoctorId = null,

                AiSymptomLogId = dto.AiSymptomLogId,

                Status = "active",

                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(session);

            await _repository.SaveChangesAsync();

            var createdSession = await _repository.GetByIdAsync(session.Id);

            if (createdSession == null)
            {
                throw new Exception(
                    "Failed to retrieve created chat session.");
            }

            return MapToDto(createdSession);
        }

        public async Task<bool> AssignDoctorAsync(Guid id, Guid doctorId)
        {
            var session = await _repository.GetByIdAsync(id);

            if (session == null)
            {
                return false;
            }

            // Kiểm tra DoctorProfile
            var doctorExists = await _context.DoctorProfiles
                .AnyAsync(d => d.Id == doctorId);

            if (!doctorExists)
            {
                throw new Exception("Doctor profile not found.");
            }

            if (session.Status == "closed")
            {
                throw new Exception(
                    "Cannot assign a doctor to a closed chat session.");
            }

            session.DoctorId = doctorId;

            session.Status = "active";

            await _repository.UpdateAsync(session);

            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateStatusAsync(Guid id, UpdateChatSessionStatusDto dto)
        {
            var session = await _repository.GetByIdAsync(id);

            if (session == null)
            {
                return false;
            }

            var status = dto.Status?.Trim().ToLower();

            if (status != "active" && status != "closed")
            {
                throw new Exception(
                    "Status must be either 'active' or 'closed'.");
            }

            session.Status = status;

            await _repository.UpdateAsync(session);

            await _repository.SaveChangesAsync();

            return true;
        }

        private static ChatSessionDto MapToDto(ChatSession session)
        {
            return new ChatSessionDto
            {
                Id = session.Id,

                PatientId = session.PatientId,

                DoctorId = session.DoctorId,

                AiSymptomLogId = session.AiSymptomLogId,

                Status = session.Status,

                CreatedAt = session.CreatedAt
            };
        }
        public async Task<Guid?> GetPatientIdByUserIdAsync(Guid userId)
        {
            var patient = await _context.PatientProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);

            return patient?.Id;
        }

        public async Task<Guid?> GetDoctorIdByUserIdAsync(Guid userId)
        {
            var doctor = await _context.DoctorProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.UserId == userId);

            return doctor?.Id;
        }
    }
}