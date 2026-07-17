using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();

        Task<User?> GetByIdAsync(Guid id);

        Task<User?> GetByPhoneAsync(string phone);

        Task<User?> GetByEmailAsync(string email);

        Task AddAsync(User user);

        Task UpdateAsync(User user);

        Task DeleteAsync(User user);

        Task SaveChangesAsync();
    }
}
