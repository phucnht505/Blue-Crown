using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class HealthGoalRepository : IHealthGoalRepository
    {
        private readonly BlueCrownContext _context;

        public HealthGoalRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task<List<HealthGoal>> GetByPatientIdAsync(Guid patientId)
        {
            return await _context.HealthGoals
                .Include(x => x.MetricType)
                .Where(x => x.PatientId == patientId)
                .OrderByDescending(x => x.StartDate)
                .ToListAsync();
        }

        public async Task<HealthGoal?> GetByIdAsync(Guid id)
        {
            return await _context.HealthGoals
                .Include(x => x.MetricType)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(HealthGoal healthGoal)
        {
            await _context.HealthGoals.AddAsync(healthGoal);
        }

        public Task UpdateAsync(HealthGoal healthGoal)
        {
            _context.HealthGoals.Update(healthGoal);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(HealthGoal healthGoal)
        {
            _context.HealthGoals.Remove(healthGoal);
            return Task.CompletedTask;
        }

        public async Task<bool> MetricTypeExistsAsync(int metricTypeId)
        {
            return await _context.MetricTypes.AnyAsync(x => x.Id == metricTypeId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}