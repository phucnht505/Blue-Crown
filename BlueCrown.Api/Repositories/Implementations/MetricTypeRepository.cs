using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class MetricTypeRepository : IMetricTypeRepository
    {
        private readonly BlueCrownContext _context;

        public MetricTypeRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task<List<MetricType>> GetAllAsync()
        {
            return await _context.MetricTypes
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<MetricType?> GetByIdAsync(int id)
        {
            return await _context.MetricTypes
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<MetricType?> GetByCodeAsync(string code)
        {
            var normalizedCode = code.Trim().ToUpper();

            return await _context.MetricTypes
                .FirstOrDefaultAsync(x => x.Code.ToUpper() == normalizedCode);
        }

        public async Task<bool> HasHealthGoalsAsync(int metricTypeId)
        {
            return await _context.HealthGoals
                .AnyAsync(x => x.MetricTypeId == metricTypeId);
        }

        public async Task<bool> HasHealthMetricsAsync(int metricTypeId)
        {
            return await _context.HealthMetrics
                .AnyAsync(x => x.MetricTypeId == metricTypeId);
        }

        public async Task AddAsync(MetricType metricType)
        {
            await _context.MetricTypes.AddAsync(metricType);
        }

        public async Task UpdateAsync(MetricType metricType)
        {
            _context.MetricTypes.Update(metricType);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(MetricType metricType)
        {
            _context.MetricTypes.Remove(metricType);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}