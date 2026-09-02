using BlueCrown.Api.DTOs.MedicalRecords;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _repository;
        private readonly IPatientProfileRepository _patientProfileRepository;
        private readonly IDoctorProfileRepository _doctorProfileRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public MedicalRecordService(IMedicalRecordRepository repository, IPatientProfileRepository patientProfileRepository, IDoctorProfileRepository doctorProfileRepository, IAppointmentRepository appointmentRepository)
        {
            _repository = repository;
            _patientProfileRepository = patientProfileRepository;
            _doctorProfileRepository = doctorProfileRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<List<MedicalRecordDto>> GetPatientRecordsAsync(Guid userId)
        {
            var patientProfile = await GetPatientProfileAsync(userId);
            var records = await _repository.GetByPatientIdAsync(patientProfile.Id);

            return records.Select(MapToDto).ToList();
        }

        public async Task<List<MedicalRecordDto>> GetDoctorRecordsAsync(Guid userId)
        {
            var doctorProfile = await GetDoctorProfileAsync(userId);
            var records = await _repository.GetByDoctorIdAsync(doctorProfile.Id);

            return records.Select(MapToDto).ToList();
        }

        public async Task<MedicalRecordDto?> GetPatientRecordByIdAsync(Guid id, Guid userId)
        {
            var patientProfile = await GetPatientProfileAsync(userId);
            var record = await _repository.GetByIdAsync(id);

            // BR-MR-007: Patient chỉ được xem MedicalRecord của chính mình.
            if (record == null || record.PatientId != patientProfile.Id)
                return null;

            return MapToDto(record);
        }

        public async Task<MedicalRecordDto?> GetDoctorRecordByIdAsync(Guid id, Guid userId)
        {
            var doctorProfile = await GetDoctorProfileAsync(userId);
            var record = await _repository.GetByIdAsync(id);

            // BR-MR-008: Doctor chỉ được xem MedicalRecord do chính mình phụ trách.
            if (record == null || record.DoctorId != doctorProfile.Id)
                return null;

            return MapToDto(record);
        }

        public async Task<MedicalRecordDto?> GetDoctorRecordByAppointmentAsync(Guid appointmentId, Guid userId)
        {
            var doctorProfile = await GetDoctorProfileAsync(userId);
            var record = await _repository.GetByAppointmentIdAsync(appointmentId);

            if (record == null || record.DoctorId != doctorProfile.Id)
                return null;

            return MapToDto(record);
        }

        public async Task<MedicalRecordDto> CreateAsync(Guid userId, CreateMedicalRecordDto dto)
        {
            var doctorProfile = await GetDoctorProfileAsync(userId);
            var appointment = await _appointmentRepository.GetByIdAsync(dto.AppointmentId);

            // BR-MR-003: Appointment phải tồn tại.
            if (appointment == null)
                throw new ArgumentException("Không tìm thấy lịch khám.");

            // BR-MR-004: Doctor chỉ tạo bệnh án cho Appointment của chính mình.
            if (appointment.DoctorId != doctorProfile.Id)
                throw new InvalidOperationException("Bạn không có quyền tạo bệnh án cho lịch khám này.");

            // BR-MR-009: MedicalRecord chỉ được tạo cho lịch khám trực tiếp.
            if (!string.Equals(appointment.Type, "clinic_visit", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Hồ sơ bệnh án chỉ được tạo cho lịch khám trực tiếp.");

            // BR-MR-005: Chỉ Appointment completed mới được tạo MedicalRecord.
            if (!string.Equals(appointment.Status, "completed", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Chỉ lịch khám đã hoàn thành mới được tạo hồ sơ bệnh án.");

            // BR-MR-006: Mỗi Appointment chỉ có một MedicalRecord.
            var existingRecord = await _repository.GetByAppointmentIdAsync(appointment.Id);

            if (existingRecord != null)
                throw new InvalidOperationException("Lịch khám này đã có hồ sơ bệnh án.");

            var diagnosis = dto.Diagnosis.Trim();
            var notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();

            var record = new MedicalRecord
            {
                Id = Guid.NewGuid(),
                AppointmentId = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = doctorProfile.Id,
                Diagnosis = diagnosis,
                Notes = notes,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(record);
            await _repository.SaveChangesAsync();

            var created = await _repository.GetByIdAsync(record.Id);

            if (created == null)
                throw new Exception("Không thể lấy hồ sơ bệnh án vừa tạo.");

            return MapToDto(created);
        }

        public async Task<MedicalRecordDto?> UpdateAsync(Guid id, Guid userId, UpdateMedicalRecordDto dto)
        {
            var doctorProfile = await GetDoctorProfileAsync(userId);
            var record = await _repository.GetByIdAsync(id);

            // BR-MR-008: Doctor chỉ được sửa MedicalRecord của chính mình.
            if (record == null || record.DoctorId != doctorProfile.Id)
                return null;

            // BR-MR-009: MedicalRecord chỉ thuộc lịch khám trực tiếp.
            if (!string.Equals(record.Appointment?.Type, "clinic_visit", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Hồ sơ bệnh án chỉ được sử dụng cho lịch khám trực tiếp.");

            record.Diagnosis = dto.Diagnosis.Trim();
            record.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();

            await _repository.UpdateAsync(record);
            await _repository.SaveChangesAsync();

            return MapToDto(record);
        }

        // BR-MR-002: Patient phải có PatientProfile.
        private async Task<PatientProfile> GetPatientProfileAsync(Guid userId)
        {
            var patientProfile = await _patientProfileRepository.GetByUserIdAsync(userId);

            if (patientProfile == null)
                throw new InvalidOperationException("Bạn chưa có hồ sơ sức khỏe.");

            return patientProfile;
        }

        // BR-MR-001: Doctor phải có DoctorProfile.
        private async Task<DoctorProfile> GetDoctorProfileAsync(Guid userId)
        {
            var doctorProfile = await _doctorProfileRepository.GetByUserIdAsync(userId);

            if (doctorProfile == null)
                throw new InvalidOperationException("Bạn chưa có hồ sơ bác sĩ.");

            return doctorProfile;
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

        private static MedicalRecordDto MapToDto(MedicalRecord record)
        {
            return new MedicalRecordDto
            {
                Id = record.Id,
                AppointmentId = record.AppointmentId,
                PatientId = record.PatientId,
                PatientName = record.Patient.User.FullName,
                DoctorId = record.DoctorId,
                DoctorName = record.Doctor.User.FullName,
                DoctorSpecialty = record.Doctor.Specialty,
                AppointmentScheduledAt = AsUtc(record.Appointment?.ScheduledAt),
                AppointmentType = record.Appointment?.Type,
                Diagnosis = record.Diagnosis,
                Notes = record.Notes,
                CreatedAt = AsUtc(record.CreatedAt)
            };
        }
    }
}