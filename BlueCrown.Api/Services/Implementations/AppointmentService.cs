using BlueCrown.Api.DTOs.Appointments;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientProfileRepository _patientProfileRepository;
        private readonly IDoctorProfileRepository _doctorProfileRepository;
        private readonly IChatSessionRepository _chatSessionRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository, IPatientProfileRepository patientProfileRepository, IDoctorProfileRepository doctorProfileRepository, IChatSessionRepository chatSessionRepository)
        {
            _appointmentRepository = appointmentRepository;
            _patientProfileRepository = patientProfileRepository;
            _doctorProfileRepository = doctorProfileRepository;
            _chatSessionRepository = chatSessionRepository;
        }

        public async Task<List<AppointmentDto>> GetMyAppointmentsAsync(Guid userId)
        {
            var patientProfile = await GetPatientProfileAsync(userId);
            var appointments = await _appointmentRepository.GetByPatientIdAsync(patientProfile.Id);
            return appointments.Select(MapToDto).ToList();
        }

        public async Task<List<AppointmentDoctorDto>> GetBookableDoctorsAsync()
        {
            var doctors = await _doctorProfileRepository.GetBookableAsync();

            return doctors.Select(doctor => new AppointmentDoctorDto
            {
                Id = doctor.Id,
                FullName = doctor.User.FullName,
                Specialty = doctor.Specialty,
                ClinicName = doctor.Clinic?.Name,
                YearsExperience = doctor.YearsExperience,
                ConsultationFee = doctor.ConsultationFee,
                RatingAvg = doctor.RatingAvg
            }).ToList();
        }

        public async Task<AppointmentDto?> GetByIdAsync(Guid id, Guid userId)
        {
            var patientProfile = await GetPatientProfileAsync(userId);
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            // BR-APT-007: Patient chỉ được xem lịch khám của chính mình.
            if (appointment == null || appointment.PatientId != patientProfile.Id)
                return null;

            return MapToDto(appointment);
        }

        public async Task<AppointmentDto> CreateAsync(Guid userId, CreateAppointmentDto dto)
        {
            var patientProfile = await GetPatientProfileAsync(userId);

            if (dto.DoctorId == Guid.Empty)
                throw new ArgumentException("Vui lòng chọn bác sĩ.");

            var doctor = await _doctorProfileRepository.GetByIdAsync(dto.DoctorId);

            // BR-APT-002: Chỉ Doctor đang hoạt động và đã xác minh mới được nhận lịch.
            if (doctor == null || doctor.LicenseVerified != true || !string.Equals(doctor.User.Status, "active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Bác sĩ không tồn tại hoặc hiện không nhận lịch.");

            var type = dto.Type.Trim().ToLowerInvariant();

            // BR-APT-004: Loại lịch khám phải thuộc danh sách hệ thống hỗ trợ.
            if (type != "online_consult" && type != "clinic_visit")
                throw new ArgumentException("Hình thức khám không hợp lệ.");

            var scheduledAtUtc = ConvertInputToUtc(dto.ScheduledAt);

            // BR-APT-003: Không được đặt lịch trong quá khứ.
            if (scheduledAtUtc <= DateTime.UtcNow)
                throw new ArgumentException("Thời gian khám phải nằm trong tương lai.");

            // BR-APT-005: Doctor không được có hai lịch cùng thời điểm.
            if (await _appointmentRepository.HasDoctorScheduleConflictAsync(doctor.Id, scheduledAtUtc))
                throw new InvalidOperationException("Bác sĩ đã có lịch khám vào thời điểm này.");

            // BR-APT-006: Patient không được có hai lịch cùng thời điểm.
            if (await _appointmentRepository.HasPatientScheduleConflictAsync(patientProfile.Id, scheduledAtUtc))
                throw new InvalidOperationException("Bạn đã có một lịch khám vào thời điểm này.");

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                ChatSessionId = null,
                PatientId = patientProfile.Id,
                DoctorId = doctor.Id,
                ScheduledAt = scheduledAtUtc,
                Type = type,
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };

            await _appointmentRepository.AddAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            var created = await _appointmentRepository.GetByIdAsync(appointment.Id);

            if (created == null)
                throw new Exception("Không thể lấy lịch khám vừa tạo.");

            return MapToDto(created);
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            var patientProfile = await GetPatientProfileAsync(userId);
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            // BR-APT-007: Patient chỉ được xóa lịch khám của chính mình.
            if (appointment == null || appointment.PatientId != patientProfile.Id)
                return false;

            // BR-APT-008: Chỉ lịch pending trong tương lai mới được xóa.
            if (!string.Equals(appointment.Status, "pending", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Chỉ lịch đang chờ xác nhận mới có thể hủy.");

            if (AsUtc(appointment.ScheduledAt) <= DateTime.UtcNow)
                throw new InvalidOperationException("Không thể hủy lịch khám đã hoặc đang diễn ra.");

            await _appointmentRepository.DeleteAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            return true;
        }

        public async Task<List<AppointmentDto>> GetDoctorAppointmentsAsync(Guid userId)
        {
            var doctorProfile = await GetDoctorProfileAsync(userId);
            var appointments = await _appointmentRepository.GetByDoctorIdAsync(doctorProfile.Id);
            return appointments.Select(MapToDto).ToList();
        }

        public async Task<AppointmentDto?> UpdateDoctorStatusAsync(Guid id, Guid userId, UpdateAppointmentStatusDto dto)
        {
            var doctorProfile = await GetDoctorProfileAsync(userId);
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            // BR-APT-009: Doctor chỉ được xử lý lịch khám được đặt với chính mình.
            if (appointment == null || appointment.DoctorId != doctorProfile.Id)
                return null;

            var currentStatus = appointment.Status?.Trim().ToLowerInvariant() ?? "pending";
            var newStatus = dto.Status.Trim().ToLowerInvariant();
            var isOnlineConsult = string.Equals(appointment.Type, "online_consult", StringComparison.OrdinalIgnoreCase);

            // BR-APT-010: Lịch pending chỉ được chuyển sang confirmed hoặc cancelled.
            if (currentStatus == "pending" && newStatus != "confirmed" && newStatus != "cancelled")
                throw new InvalidOperationException("Lịch đang chờ chỉ có thể được xác nhận hoặc từ chối.");

            // BR-APT-015: Không được xác nhận lịch đã qua thời gian hẹn.
            if (currentStatus == "pending" && newStatus == "confirmed" && AsUtc(appointment.ScheduledAt) <= DateTime.UtcNow)
                throw new InvalidOperationException("Không thể xác nhận lịch khám đã qua thời gian hẹn.");

            // BR-APT-011: Lịch confirmed chỉ được completed hoặc cancelled.
            if (currentStatus == "confirmed" && newStatus != "completed" && newStatus != "cancelled")
                throw new InvalidOperationException("Lịch đã xác nhận chỉ có thể được hoàn thành hoặc hủy.");

            // BR-APT-012: completed và cancelled là trạng thái cuối.
            if (currentStatus == "completed")
                throw new InvalidOperationException("Lịch khám đã hoàn thành và không thể thay đổi trạng thái.");

            if (currentStatus == "cancelled")
                throw new InvalidOperationException("Lịch khám đã bị hủy và không thể thay đổi trạng thái.");

            // BR-APT-016: Online consultation chỉ completed khi Doctor kết thúc Chat.
            if (isOnlineConsult && newStatus == "completed")
                throw new InvalidOperationException("Tư vấn trực tuyến phải được kết thúc trong phòng Chat.");

            // BR-APT-013: Khám trực tiếp chỉ được completed sau khi đã đến giờ.
            if (!isOnlineConsult && newStatus == "completed" && AsUtc(appointment.ScheduledAt) > DateTime.UtcNow)
                throw new InvalidOperationException("Chưa đến thời gian khám nên không thể đánh dấu hoàn thành.");

            if (isOnlineConsult && currentStatus == "pending" && newStatus == "confirmed")
                await LinkOnlineChatSessionAsync(appointment, doctorProfile.Id);

            if (isOnlineConsult && currentStatus == "confirmed" && newStatus == "cancelled")
                await CloseLinkedChatSessionAsync(appointment);

            appointment.Status = newStatus;
            await _appointmentRepository.SaveChangesAsync();

            return MapToDto(appointment);
        }

        private async Task LinkOnlineChatSessionAsync(Appointment appointment, Guid doctorId)
        {
            // BR-CHAT-006: Một Appointment online chỉ liên kết một ChatSession.
            if (appointment.ChatSessionId.HasValue)
            {
                var linkedSession = await _chatSessionRepository.GetByIdAsync(appointment.ChatSessionId.Value);

                if (linkedSession == null)
                    throw new InvalidOperationException("Chat Session của lịch tư vấn không tồn tại.");

                if (linkedSession.DoctorId.HasValue && linkedSession.DoctorId.Value != doctorId)
                    throw new InvalidOperationException("Chat Session này đã thuộc bác sĩ khác.");

                linkedSession.DoctorId = doctorId;
                linkedSession.Status = "active";
                await _chatSessionRepository.UpdateAsync(linkedSession);
                return;
            }

            var patientSessions = await _chatSessionRepository.GetByPatientIdAsync(appointment.PatientId);

            var reusableSession = patientSessions
                .Where(session => string.Equals(session.Status, "active", StringComparison.OrdinalIgnoreCase))
                .Where(session => session.Appointments.Count == 0)
                .FirstOrDefault(session => !session.DoctorId.HasValue || session.DoctorId.Value == doctorId);

            var otherActiveSession = patientSessions.Any(session =>
                string.Equals(session.Status, "active", StringComparison.OrdinalIgnoreCase) &&
                session.Id != reusableSession?.Id);

            // BR-CHAT-007: Patient chỉ có một cuộc tư vấn đang hoạt động.
            if (reusableSession == null && otherActiveSession)
                throw new InvalidOperationException("Bệnh nhân đang có một cuộc tư vấn khác chưa kết thúc.");

            if (reusableSession != null)
            {
                reusableSession.DoctorId = doctorId;
                reusableSession.Status = "active";
                appointment.ChatSessionId = reusableSession.Id;
                await _chatSessionRepository.UpdateAsync(reusableSession);
                return;
            }

            var newSession = new ChatSession
            {
                Id = Guid.NewGuid(),
                PatientId = appointment.PatientId,
                DoctorId = doctorId,
                AiSymptomLogId = null,
                Status = "active",
                CreatedAt = DateTime.UtcNow
            };

            appointment.ChatSessionId = newSession.Id;
            await _chatSessionRepository.AddAsync(newSession);
        }

        private async Task CloseLinkedChatSessionAsync(Appointment appointment)
        {
            if (!appointment.ChatSessionId.HasValue)
                return;

            var session = await _chatSessionRepository.GetByIdAsync(appointment.ChatSessionId.Value);

            if (session == null)
                return;

            // BR-CHAT-008: Hủy lịch online thì ChatSession liên quan cũng phải đóng.
            if (!string.Equals(session.Status, "closed", StringComparison.OrdinalIgnoreCase))
            {
                session.Status = "closed";
                await _chatSessionRepository.UpdateAsync(session);
            }
        }

        // BR-APT-001: User phải có PatientProfile trước khi sử dụng lịch khám với vai trò Patient.
        private async Task<PatientProfile> GetPatientProfileAsync(Guid userId)
        {
            var patientProfile = await _patientProfileRepository.GetByUserIdAsync(userId);

            if (patientProfile == null)
                throw new InvalidOperationException("Bạn cần tạo hồ sơ sức khỏe trước khi đặt lịch khám.");

            return patientProfile;
        }

        // BR-APT-014: Doctor phải có DoctorProfile trước khi quản lý lịch khám.
        private async Task<DoctorProfile> GetDoctorProfileAsync(Guid userId)
        {
            var doctorProfile = await _doctorProfileRepository.GetByUserIdAsync(userId);

            if (doctorProfile == null)
                throw new InvalidOperationException("Bạn chưa có hồ sơ bác sĩ.");

            return doctorProfile;
        }

        private static DateTime ConvertInputToUtc(DateTime value)
        {
            if (value.Kind == DateTimeKind.Utc)
                return value;

            if (value.Kind == DateTimeKind.Local)
                return value.ToUniversalTime();

            var vietnamTimeZone = GetVietnamTimeZone();
            return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(value, DateTimeKind.Unspecified), vietnamTimeZone);
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

        private static TimeZoneInfo GetVietnamTimeZone()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            }
        }

        private static AppointmentDto MapToDto(Appointment appointment)
        {
            return new AppointmentDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                PatientName = appointment.Patient.User.FullName,
                DoctorId = appointment.DoctorId,
                DoctorName = appointment.Doctor.User.FullName,
                DoctorSpecialty = appointment.Doctor.Specialty,
                ClinicName = appointment.Doctor.Clinic?.Name,
                // BR-APT-017: Tư vấn trực tuyến được miễn phí.
                ConsultationFee = string.Equals(appointment.Type, "online_consult", StringComparison.OrdinalIgnoreCase) ? 0 : appointment.Doctor.ConsultationFee,
                ScheduledAt = AsUtc(appointment.ScheduledAt),
                Type = appointment.Type,
                Status = appointment.Status,
                CreatedAt = AsUtc(appointment.CreatedAt)
            };
        }
    }
}