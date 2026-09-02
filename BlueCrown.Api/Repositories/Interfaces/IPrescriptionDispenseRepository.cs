using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IPrescriptionDispenseRepository
    {
        Task AddRangeAsync(IEnumerable<PrescriptionDispenseItem> items);
    }
}