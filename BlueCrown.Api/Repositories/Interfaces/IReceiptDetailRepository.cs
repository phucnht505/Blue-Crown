using BlueCrown.Api.Models;

namespace BlueCrown.Api.Repositories.Interfaces
{
    public interface IReceiptDetailRepository
    {
        Task<List<ReceiptDetail>> GetFefoDetailsAsync(Guid productId);
    }
}