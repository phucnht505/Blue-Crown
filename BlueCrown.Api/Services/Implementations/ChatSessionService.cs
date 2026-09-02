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
            var sessions = await _repository.GetByPatientIdAsync(patientId);
            return sessions.Select(MapToDto).ToList();
        }

        public async Task<List<ChatSessionDto>> GetDoctorSessionsAsync(Guid doctorId)
        {
            var sessions = await _repository.GetByDoctorIdAsync(doctorId);
            return sessions.Select(MapToDto).ToList();
        }

        public async Task<List<ChatSessionDto>> GetAvailableSessionsAsync()
        {
            var sessions = await _repository.GetUnassignedAsync();
            return sessions.Select(MapToDto).ToList();
        }

        public async Task<ChatSessionDto?> GetByIdAsync(Guid id, Guid userId)
        {
            var session = await _repository.GetByIdAsync(id);

            if (session == null)
                return null;

            await EnsureUserCanAccessSessionAsync(session, userId);
            return MapToDto(session);
        }

        public async Task<ChatSessionDto> CreateAsync(Guid patientId, CreateChatSessionDto dto)
        {
            var patientExists = await _context.PatientProfiles.AnyAsync(p => p.Id == patientId);

            if (!patientExists)
                throw new Exception("Không tìm thấy Patient Profile.");

            var hasActiveSession = await _context.ChatSessions.AnyAsync(x => x.PatientId == patientId && x.Status == "active");

            if (hasActiveSession)
                throw new Exception("Bạn đang có một cuộc tư vấn chưa kết thúc. Vui lòng tiếp tục hoặc đóng cuộc trò chuyện hiện tại.");

            if (dto.AiSymptomLogId.HasValue)
            {
                var symptomLogExists = await _context.SymptomLogs.AnyAsync(x => x.Id == dto.AiSymptomLogId.Value && x.PatientId == patientId);

                if (!symptomLogExists)
                    throw new Exception("Không tìm thấy AI Symptom Log phù hợp với Patient.");
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
                throw new Exception("Không thể lấy Chat Session vừa tạo.");

            return MapToDto(createdSession);
        }

        public async Task<bool> AssignDoctorAsync(Guid id, Guid doctorId)
        {
            var session = await _repository.GetByIdAsync(id);

            if (session == null)
                return false;

            var doctorExists = await _context.DoctorProfiles.AnyAsync(d => d.Id == doctorId);

            if (!doctorExists)
                throw new Exception("Không tìm thấy Doctor Profile.");

            if (string.Equals(session.Status, "closed", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Cuộc tư vấn đã đóng.");

            var appointment = GetLinkedOnlineAppointment(session);

            // BR-CHAT-001: Nếu Chat thuộc lịch online đã chọn bác sĩ thì chỉ bác sĩ đó được tiếp nhận.
            if (appointment != null && appointment.DoctorId != doctorId)
                throw new InvalidOperationException("Cuộc tư vấn này thuộc lịch hẹn của một bác sĩ khác.");

            // BR-CHAT-002: Lịch online phải được xác nhận trước khi bắt đầu tư vấn.
            if (appointment != null && !string.Equals(appointment.Status, "confirmed", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Lịch tư vấn trực tuyến chưa ở trạng thái đã xác nhận.");

            if (session.DoctorId.HasValue)
            {
                if (session.DoctorId.Value == doctorId)
                    return true;

                throw new Exception("Cuộc tư vấn này đã được bác sĩ khác tiếp nhận.");
            }

            session.DoctorId = doctorId;
            session.Status = "active";

            await _repository.UpdateAsync(session);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateStatusAsync(Guid id, Guid userId, UpdateChatSessionStatusDto dto)
        {
            var session = await _repository.GetByIdAsync(id);

            if (session == null)
                return false;

            await EnsureUserCanAccessSessionAsync(session, userId);

            var status = dto.Status?.Trim().ToLowerInvariant();

            if (status != "active" && status != "closed")
                throw new Exception("Status chỉ được là 'active' hoặc 'closed'.");

            var currentStatus = session.Status?.Trim().ToLowerInvariant();

            if (currentStatus == "closed" && status == "active")
                throw new Exception("Cuộc tư vấn đã đóng không thể mở lại.");

            if (status == "closed")
                await CompleteLinkedOnlineAppointmentAsync(session, userId);

            if (currentStatus == status)
                return true;

            session.Status = status;

            await _repository.UpdateAsync(session);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<Guid?> GetPatientIdByUserIdAsync(Guid userId)
        {
            return await _context.PatientProfiles
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .Select(p => (Guid?)p.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<Guid?> GetDoctorIdByUserIdAsync(Guid userId)
        {
            return await _context.DoctorProfiles
                .AsNoTracking()
                .Where(d => d.UserId == userId)
                .Select(d => (Guid?)d.Id)
                .FirstOrDefaultAsync();
        }

        private async Task CompleteLinkedOnlineAppointmentAsync(ChatSession session, Guid userId)
        {
            var appointment = GetLinkedOnlineAppointment(session);

            if (appointment == null)
                return;

            var doctorId = await GetDoctorIdByUserIdAsync(userId);

            // BR-CHAT-003: Chỉ Doctor phụ trách mới được kết thúc lịch tư vấn online.
            if (!doctorId.HasValue || session.DoctorId != doctorId.Value || appointment.DoctorId != doctorId.Value)
                throw new UnauthorizedAccessException("Chỉ bác sĩ phụ trách mới được kết thúc tư vấn trực tuyến.");

            var appointmentStatus = appointment.Status?.Trim().ToLowerInvariant();

            if (appointmentStatus == "completed")
                return;

            if (appointmentStatus == "cancelled")
                throw new InvalidOperationException("Lịch tư vấn đã bị hủy.");

            // BR-CHAT-004: Online consultation phải confirmed trước khi completed.
            if (appointmentStatus != "confirmed")
                throw new InvalidOperationException("Lịch tư vấn trực tuyến phải được xác nhận trước khi kết thúc.");

            // BR-CHAT-005: Không được hoàn tất trước thời gian hẹn.
            if (AsUtc(appointment.ScheduledAt) > DateTime.UtcNow)
                throw new InvalidOperationException("Chưa đến thời gian tư vấn nên chưa thể kết thúc lịch.");

            appointment.Status = "completed";
        }

        private async Task EnsureUserCanAccessSessionAsync(ChatSession session, Guid userId)
        {
            var patientId = await GetPatientIdByUserIdAsync(userId);

            if (patientId.HasValue && session.PatientId == patientId.Value)
                return;

            var doctorId = await GetDoctorIdByUserIdAsync(userId);

            if (doctorId.HasValue && session.DoctorId == doctorId.Value)
                return;

            throw new UnauthorizedAccessException("Bạn không có quyền truy cập cuộc tư vấn này.");
        }

        private static Appointment? GetLinkedOnlineAppointment(ChatSession session)
        {
            return session.Appointments
                .Where(a => string.Equals(a.Type, "online_consult", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefault();
        }

        private static DateTime AsUtc(DateTime value)
        {
            return value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        private static DateTime? AsUtc(DateTime? value)
        {
            if (!value.HasValue)
                return null;

            return AsUtc(value.Value);
        }

        private static ChatSessionDto MapToDto(ChatSession session)
        {
            var appointment = GetLinkedOnlineAppointment(session);

            return new ChatSessionDto
            {
                Id = session.Id,
                PatientId = session.PatientId,
                PatientName = session.Patient.User.FullName,
                DoctorId = session.DoctorId,
                DoctorName = session.Doctor?.User.FullName,
                AiSymptomLogId = session.AiSymptomLogId,
                AppointmentId = appointment?.Id,
                AppointmentType = appointment?.Type,
                AppointmentStatus = appointment?.Status,
                AppointmentScheduledAt = appointment == null ? null : AsUtc(appointment.ScheduledAt),
                Status = session.Status,
                CreatedAt = AsUtc(session.CreatedAt)
            };
        }
    }
}