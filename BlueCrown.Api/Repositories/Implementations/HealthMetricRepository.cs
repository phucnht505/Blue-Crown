using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlueCrown.Api.Repositories.Implementations
{
    public class HealthMetricRepository : IHealthMetricRepository
    {
        private readonly BlueCrownContext _context;

        public HealthMetricRepository(BlueCrownContext context)
        {
            _context = context;
        }

        public async Task<List<HealthMetric>> GetByPatientIdAsync(Guid patientId)
        {
            return await _context.HealthMetrics
                .Include(x => x.MetricType)
                .Where(x => x.PatientId == patientId)
                .OrderByDescending(x => x.RecordedAt)
                .ToListAsync();
        }

        public async Task<HealthMetric?> GetByIdAsync(Guid id)
        {
            return await _context.HealthMetrics
                .Include(x => x.MetricType)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<HealthMetric?> GetLatestAsync(Guid patientId)
        {
            return await _context.HealthMetrics
                .Include(x => x.MetricType)
                .Where(x => x.PatientId == patientId)
                .OrderByDescending(x => x.RecordedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> MetricTypeExistsAsync(int metricTypeId)
        {
            return await _context.MetricTypes
                .AnyAsync(x => x.Id == metricTypeId);
        }

        public async Task<MetricType?> GetMetricTypeAsync(int metricTypeId)
        {
            return await _context.MetricTypes
                .FirstOrDefaultAsync(x => x.Id == metricTypeId);
        }

        // THÊM MỚI: không hard-code MetricType ở Angular.
        public async Task<List<MetricType>> GetMetricTypesAsync()
        {
            return await _context.MetricTypes
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task AddAsync(HealthMetric healthMetric)
        {
            await _context.HealthMetrics.AddAsync(healthMetric);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}