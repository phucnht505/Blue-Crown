using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.Users
{
    public class LoginDto
    {
        public string UserName { get; set; } = string.Empty;
        
        public string Password { get; set; } = string.Empty;
    }
}
