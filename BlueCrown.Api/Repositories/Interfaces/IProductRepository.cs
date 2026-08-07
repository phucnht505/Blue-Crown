using BlueCrown.Api.Models;
namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
        Task<IEnumerable<Product>> SearchAsync(string keyword);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task SaveChangesAsync();
    }
}
