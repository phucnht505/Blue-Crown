using BlueCrown.Api.DTOs.MedicalRecords;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _repository;

        public MedicalRecordService(IMedicalRecordRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MedicalRecordDto>> GetAllAsync()
        {
            var records = await _repository.GetAllAsync();

            return records.Select(MapToDto).ToList();
        }

        public async Task<MedicalRecordDto?> GetByIdAsync(Guid id)
        {
            var record = await _repository.GetByIdAsync(id);

            return record == null ? null : MapToDto(record);
        }

        public async Task<List<MedicalRecordDto>> GetByPatientIdAsync(Guid patientId)
        {
            var records = await _repository.GetByPatientIdAsync(patientId);

            return records.Select(MapToDto).ToList();
        }

        public async Task<List<MedicalRecordDto>> GetByDoctorIdAsync(Guid doctorId)
        {
            var records = await _repository.GetByDoctorIdAsync(doctorId);

            return records.Select(MapToDto).ToList();
        }

        public async Task<MedicalRecordDto?> GetByAppointmentIdAsync(Guid appointmentId)
        {
            var record = await _repository.GetByAppointmentIdAsync(appointmentId);

            return record == null ? null : MapToDto(record);
        }

        public async Task<MedicalRecordDto> CreateAsync(CreateMedicalRecordDto dto)
        {
            var record = new MedicalRecord
            {
                Id = Guid.NewGuid(),
                AppointmentId = dto.AppointmentId,
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                Diagnosis = dto.Diagnosis,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(record);
            await _repository.SaveChangesAsync();

            return MapToDto(record);
        }

        public async Task<MedicalRecordDto?> UpdateAsync(Guid id, CreateMedicalRecordDto dto)
        {
            var record = await _repository.GetByIdAsync(id);

            if (record == null)
                return null;

            record.AppointmentId = dto.AppointmentId;
            record.PatientId = dto.PatientId;
            record.DoctorId = dto.DoctorId;
            record.Diagnosis = dto.Diagnosis;
            record.Notes = dto.Notes;

            await _repository.UpdateAsync(record);
            await _repository.SaveChangesAsync();

            return MapToDto(record);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var record = await _repository.GetByIdAsync(id);

            if (record == null)
                return false;

            await _repository.DeleteAsync(record);
            await _repository.SaveChangesAsync();

            return true;
        }

        private static MedicalRecordDto MapToDto(MedicalRecord record)
        {
            return new MedicalRecordDto
            {
                Id = record.Id,
                AppointmentId = record.AppointmentId,
                PatientId = record.PatientId,
                DoctorId = record.DoctorId,
                Diagnosis = record.Diagnosis,
                Notes = record.Notes,
                CreatedAt = record.CreatedAt
            };
        }
    }
}