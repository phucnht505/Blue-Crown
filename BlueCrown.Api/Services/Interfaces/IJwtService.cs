using BlueCrown.Api.Models;

namespace BlueCrown.Api.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}