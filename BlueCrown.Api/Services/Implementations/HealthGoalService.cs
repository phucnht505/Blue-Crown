using BlueCrown.Api.DTOs.HealthGoals;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class HealthGoalService : IHealthGoalService
    {
        private readonly IHealthGoalRepository _repository;
        private readonly IPatientProfileRepository _patientProfileRepository;
        private readonly IDoctorProfileRepository _doctorProfileRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public HealthGoalService(
            IHealthGoalRepository repository,
            IPatientProfileRepository patientProfileRepository,
            IDoctorProfileRepository doctorProfileRepository,
            IAppointmentRepository appointmentRepository)
        {
            _repository = repository;
            _patientProfileRepository = patientProfileRepository;
            _doctorProfileRepository = doctorProfileRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<List<HealthGoalDto>> GetMyGoalsAsync(Guid userId)
        {
            var patientProfile = await GetPatientProfileAsync(userId);
            var goals = await _repository.GetByPatientIdAsync(patientProfile.Id);

            return goals.Select(MapToDto).ToList();
        }

        public async Task<HealthGoalDto?> GetByIdAsync(Guid id, Guid userId)
        {
            var patientProfile = await GetPatientProfileAsync(userId);
            var goal = await _repository.GetByIdAsync(id);

            // BR-HG-005: Patient chỉ được xem mục tiêu sức khỏe của chính mình.
            if (goal == null || goal.PatientId != patientProfile.Id)
                return null;

            return MapToDto(goal);
        }

        public async Task<HealthGoalDto> CreateAsync(Guid userId, CreateHealthGoalDto dto)
        {
            var patientProfile = await GetPatientProfileAsync(userId);

            await ValidateGoalInputAsync(
                dto.MetricTypeId,
                dto.TargetValue,
                dto.StartDate,
                dto.EndDate
            );

            var goal = new HealthGoal
            {
                Id = Guid.NewGuid(),
                PatientId = patientProfile.Id,
                MetricTypeId = dto.MetricTypeId,
                TargetValue = dto.TargetValue,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = "active",
                CreatedByUserId = userId,
                CreatedByRole = "patient"
            };

            await _repository.AddAsync(goal);
            await _repository.SaveChangesAsync();

            var created = await _repository.GetByIdAsync(goal.Id);

            if (created == null)
                throw new InvalidOperationException("Không thể lấy mục tiêu sức khỏe vừa tạo.");

            return MapToDto(created);
        }

        public async Task<bool> UpdateAsync(Guid id, Guid userId, UpdateHealthGoalDto dto)
        {
            var patientProfile = await GetPatientProfileAsync(userId);
            var goal = await _repository.GetByIdAsync(id);

            if (goal == null || goal.PatientId != patientProfile.Id)
                return false;

            // BR-HG-007: Patient chỉ được sửa mục tiêu do chính Patient đó tạo.
            if (!string.Equals(goal.CreatedByRole, "patient", StringComparison.OrdinalIgnoreCase) ||
                goal.CreatedByUserId != userId)
            {
                throw new InvalidOperationException("Bạn không thể chỉnh sửa mục tiêu sức khỏe do bác sĩ thiết lập.");
            }

            await ValidateGoalInputAsync(
                dto.MetricTypeId,
                dto.TargetValue,
                dto.StartDate,
                dto.EndDate
            );

            goal.MetricTypeId = dto.MetricTypeId;
            goal.TargetValue = dto.TargetValue;
            goal.StartDate = dto.StartDate;
            goal.EndDate = dto.EndDate;

            if (!string.IsNullOrWhiteSpace(dto.Status))
                goal.Status = NormalizeStatus(dto.Status);

            await _repository.UpdateAsync(goal);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            var patientProfile = await GetPatientProfileAsync(userId);
            var goal = await _repository.GetByIdAsync(id);

            if (goal == null || goal.PatientId != patientProfile.Id)
                return false;

            // BR-HG-008: Patient chỉ được xóa mục tiêu do chính Patient đó tạo.
            if (!string.Equals(goal.CreatedByRole, "patient", StringComparison.OrdinalIgnoreCase) ||
                goal.CreatedByUserId != userId)
            {
                throw new InvalidOperationException("Bạn không thể xóa mục tiêu sức khỏe do bác sĩ thiết lập.");
            }

            await _repository.DeleteAsync(goal);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<List<DoctorHealthGoalPatientDto>> GetDoctorPatientsAsync(Guid userId)
        {
            var doctor = await GetDoctorProfileAsync(userId);
            var appointments = await _appointmentRepository.GetByDoctorIdAsync(doctor.Id);

            var eligibleAppointments = appointments
                .Where(x =>
                    string.Equals(x.Status, "confirmed", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(x.Status, "completed", StringComparison.OrdinalIgnoreCase))
                .ToList();

            return eligibleAppointments
                .GroupBy(x => x.PatientId)
                .Select(group =>
                {
                    var latest = group
                        .OrderByDescending(x => x.ScheduledAt)
                        .First();

                    return new DoctorHealthGoalPatientDto
                    {
                        PatientId = latest.PatientId,
                        UserId = latest.Patient.UserId,
                        FullName = latest.Patient.User.FullName,
                        LastAppointmentAt = AsUtc(latest.ScheduledAt),
                        AppointmentCount = group.Count()
                    };
                })
                .OrderBy(x => x.FullName)
                .ToList();
        }

        public async Task<List<DoctorHealthGoalMetricTypeDto>> GetDoctorMetricTypesAsync(Guid userId)
        {
            await GetDoctorProfileAsync(userId);

            var metricTypes = await _repository.GetMetricTypesAsync();

            return metricTypes.Select(x => new DoctorHealthGoalMetricTypeDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Unit = x.Unit
            }).ToList();
        }

        public async Task<List<HealthGoalDto>> GetDoctorPatientGoalsAsync(Guid userId, Guid patientId)
        {
            var doctor = await GetDoctorProfileAsync(userId);

            await EnsureDoctorPatientAccessAsync(
                doctor.Id,
                patientId
            );

            var goals = await _repository.GetByPatientIdAsync(patientId);

            return goals.Select(MapToDto).ToList();
        }

        public async Task<HealthGoalDto> CreateForPatientAsync(
            Guid userId,
            Guid patientId,
            CreateHealthGoalDto dto)
        {
            var doctor = await GetDoctorProfileAsync(userId);

            await EnsureDoctorPatientAccessAsync(
                doctor.Id,
                patientId
            );

            await ValidateGoalInputAsync(
                dto.MetricTypeId,
                dto.TargetValue,
                dto.StartDate,
                dto.EndDate
            );

            var goal = new HealthGoal
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                MetricTypeId = dto.MetricTypeId,
                TargetValue = dto.TargetValue,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = "active",
                CreatedByUserId = userId,
                CreatedByRole = "doctor"
            };

            await _repository.AddAsync(goal);
            await _repository.SaveChangesAsync();

            var created = await _repository.GetByIdAsync(goal.Id);

            if (created == null)
                throw new InvalidOperationException("Không thể tải lại mục tiêu sức khỏe vừa tạo.");

            return MapToDto(created);
        }

        public async Task<bool> UpdateForPatientAsync(
            Guid id,
            Guid userId,
            Guid patientId,
            UpdateHealthGoalDto dto)
        {
            var doctor = await GetDoctorProfileAsync(userId);

            await EnsureDoctorPatientAccessAsync(
                doctor.Id,
                patientId
            );

            var goal = await _repository.GetByIdAsync(id);

            if (goal == null || goal.PatientId != patientId)
                return false;

            // BR-HG-DOCTOR-003: Doctor chỉ được sửa Goal do chính Doctor đó tạo.
            if (!string.Equals(goal.CreatedByRole, "doctor", StringComparison.OrdinalIgnoreCase) ||
                goal.CreatedByUserId != userId)
            {
                throw new UnauthorizedAccessException("Bạn chỉ có thể chỉnh sửa mục tiêu sức khỏe do chính mình thiết lập.");
            }

            await ValidateGoalInputAsync(
                dto.MetricTypeId,
                dto.TargetValue,
                dto.StartDate,
                dto.EndDate
            );

            goal.MetricTypeId = dto.MetricTypeId;
            goal.TargetValue = dto.TargetValue;
            goal.StartDate = dto.StartDate;
            goal.EndDate = dto.EndDate;

            if (!string.IsNullOrWhiteSpace(dto.Status))
                goal.Status = NormalizeStatus(dto.Status);

            await _repository.UpdateAsync(goal);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CancelForPatientAsync(
            Guid id,
            Guid userId,
            Guid patientId)
        {
            var doctor = await GetDoctorProfileAsync(userId);

            await EnsureDoctorPatientAccessAsync(
                doctor.Id,
                patientId
            );

            var goal = await _repository.GetByIdAsync(id);

            if (goal == null || goal.PatientId != patientId)
                return false;

            // BR-HG-DOCTOR-004: Doctor chỉ được hủy Goal do chính Doctor đó tạo.
            if (!string.Equals(goal.CreatedByRole, "doctor", StringComparison.OrdinalIgnoreCase) ||
                goal.CreatedByUserId != userId)
            {
                throw new UnauthorizedAccessException("Bạn chỉ có thể hủy mục tiêu sức khỏe do chính mình thiết lập.");
            }

            // Doctor không xóa vật lý để bảo toàn lịch sử.
            goal.Status = "cancelled";

            await _repository.UpdateAsync(goal);
            await _repository.SaveChangesAsync();

            return true;
        }

        private async Task ValidateGoalInputAsync(
            int metricTypeId,
            decimal? targetValue,
            DateOnly? startDate,
            DateOnly? endDate)
        {
            if (!await _repository.MetricTypeExistsAsync(metricTypeId))
                throw new ArgumentException("Loại chỉ số sức khỏe không tồn tại.");

            if (!targetValue.HasValue || targetValue <= 0)
                throw new ArgumentException("Giá trị mục tiêu phải lớn hơn 0.");

            if (startDate.HasValue &&
                endDate.HasValue &&
                endDate < startDate)
            {
                throw new ArgumentException("Ngày kết thúc không được nhỏ hơn ngày bắt đầu.");
            }
        }

        private async Task EnsureDoctorPatientAccessAsync(
            Guid doctorId,
            Guid patientId)
        {
            if (!await _appointmentRepository.HasDoctorPatientAccessAsync(
                doctorId,
                patientId))
            {
                throw new UnauthorizedAccessException("Bạn không có quyền quản lý mục tiêu sức khỏe của bệnh nhân này.");
            }
        }

        private async Task<PatientProfile> GetPatientProfileAsync(Guid userId)
        {
            var patientProfile =
                await _patientProfileRepository.GetByUserIdAsync(userId);

            if (patientProfile == null)
                throw new InvalidOperationException("Bạn cần tạo hồ sơ sức khỏe trước khi sử dụng chức năng này.");

            return patientProfile;
        }

        private async Task<DoctorProfile> GetDoctorProfileAsync(Guid userId)
        {
            var doctorProfile =
                await _doctorProfileRepository.GetByUserIdAsync(userId);

            if (doctorProfile == null)
                throw new InvalidOperationException("Bạn chưa có hồ sơ bác sĩ.");

            return doctorProfile;
        }

        private static string NormalizeStatus(string status)
        {
            var value = status.Trim().ToLowerInvariant();

            if (value != "active" &&
                value != "completed" &&
                value != "cancelled")
            {
                throw new ArgumentException("Trạng thái mục tiêu không hợp lệ.");
            }

            return value;
        }

        private static DateTime AsUtc(DateTime value)
        {
            return value.Kind == DateTimeKind.Utc
                ? value
                : DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        private static HealthGoalDto MapToDto(HealthGoal goal)
        {
            return new HealthGoalDto
            {
                Id = goal.Id,
                PatientId = goal.PatientId,
                MetricTypeId = goal.MetricTypeId,
                MetricTypeCode = goal.MetricType.Code,
                MetricTypeName = goal.MetricType.Name,
                MetricTypeUnit = goal.MetricType.Unit,
                TargetValue = goal.TargetValue,
                StartDate = goal.StartDate,
                EndDate = goal.EndDate,
                Status = goal.Status,
                CreatedByUserId = goal.CreatedByUserId,
                CreatedByRole = goal.CreatedByRole
            };
        }
    }
}