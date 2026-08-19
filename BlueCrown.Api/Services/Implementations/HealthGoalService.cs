using BlueCrown.Api.DTOs.HealthGoals;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class HealthGoalService : IHealthGoalService
    {
        private readonly IHealthGoalRepository _repository;

        public HealthGoalService(IHealthGoalRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<HealthGoalDto>> GetMyGoalsAsync(Guid patientId)
        {
            var goals = await _repository.GetByPatientIdAsync(patientId);
            return goals.Select(MapToDto).ToList();
        }

        public async Task<HealthGoalDto?> GetByIdAsync(Guid id, Guid patientId)
        {
            var goal = await _repository.GetByIdAsync(id);

            if (goal == null || goal.PatientId != patientId)
                return null;

            return MapToDto(goal);
        }

        public async Task<HealthGoalDto> CreateAsync(Guid patientId, CreateHealthGoalDto dto)
        {
            if (!await _repository.MetricTypeExistsAsync(dto.MetricTypeId))
                throw new ArgumentException("MetricType không tồn tại.");

            if (dto.TargetValue.HasValue && dto.TargetValue <= 0)
                throw new ArgumentException("TargetValue phải lớn hơn 0.");

            if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.EndDate < dto.StartDate)
                throw new ArgumentException("EndDate không được nhỏ hơn StartDate.");

            var goal = new HealthGoal
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                MetricTypeId = dto.MetricTypeId,
                TargetValue = dto.TargetValue,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = "Active"
            };

            await _repository.AddAsync(goal);
            await _repository.SaveChangesAsync();

            var created = await _repository.GetByIdAsync(goal.Id);

            if (created == null)
                throw new Exception("Không thể lấy HealthGoal vừa tạo.");

            return MapToDto(created);
        }

        public async Task<bool> UpdateAsync(Guid id, Guid patientId, UpdateHealthGoalDto dto)
        {
            if (!await _repository.MetricTypeExistsAsync(dto.MetricTypeId))
                throw new ArgumentException("MetricType không tồn tại.");

            if (dto.TargetValue.HasValue && dto.TargetValue <= 0)
                throw new ArgumentException("TargetValue phải lớn hơn 0.");

            if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.EndDate < dto.StartDate)
                throw new ArgumentException("EndDate không được nhỏ hơn StartDate.");

            var goal = await _repository.GetByIdAsync(id);

            if (goal == null || goal.PatientId != patientId)
                return false;

            goal.MetricTypeId = dto.MetricTypeId;
            goal.TargetValue = dto.TargetValue;
            goal.StartDate = dto.StartDate;
            goal.EndDate = dto.EndDate;
            goal.Status = string.IsNullOrWhiteSpace(dto.Status) ? goal.Status : dto.Status.Trim();

            await _repository.UpdateAsync(goal);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, Guid patientId)
        {
            var goal = await _repository.GetByIdAsync(id);

            if (goal == null || goal.PatientId != patientId)
                return false;

            await _repository.DeleteAsync(goal);
            await _repository.SaveChangesAsync();

            return true;
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
                Status = goal.Status
            };
        }
    }
}