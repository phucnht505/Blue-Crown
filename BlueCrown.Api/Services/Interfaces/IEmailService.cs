namespace BlueCrown.Api.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendPasswordResetOtpAsync(string email, string fullName, string otp);
    }
}