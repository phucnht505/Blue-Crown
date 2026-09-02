using BlueCrown.Api.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace BlueCrown.Api.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public EmailService(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public async Task SendPasswordResetOtpAsync(string email, string fullName, string otp)
        {
            var host = _configuration["Email:Host"];
            var username = _configuration["Email:Username"];
            var password = _configuration["Email:Password"];
            var fromEmail = _configuration["Email:FromEmail"];
            var fromName = _configuration["Email:FromName"] ?? "Blue Crown";
            var testRecipient = _configuration["Email:TestRecipient"];
            var portText = _configuration["Email:Port"];

            if (string.IsNullOrWhiteSpace(host) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(fromEmail) ||
                !int.TryParse(portText, out var port))
                throw new InvalidOperationException("Cấu hình Email của hệ thống chưa đầy đủ.");

            var recipientEmail = _environment.IsDevelopment() && !string.IsNullOrWhiteSpace(testRecipient)
                ? testRecipient
                : email;

            using var message = new MailMessage();
            message.From = new MailAddress(fromEmail, fromName);
            message.To.Add(recipientEmail);
            message.Subject = _environment.IsDevelopment()
                ? $"[DEV] Blue Crown - OTP cho {email}"
                : "Blue Crown - Mã OTP đặt lại mật khẩu";

            message.IsBodyHtml = true;
            message.Body = $"""
                <div style="font-family:Arial,sans-serif;max-width:560px;margin:auto;padding:24px">
                    <h2 style="margin-bottom:16px">Blue Crown</h2>
                    <p>Xin chào {WebUtility.HtmlEncode(fullName)},</p>
                    <p>Bạn vừa yêu cầu đặt lại mật khẩu cho tài khoản Blue Crown.</p>

                    {(_environment.IsDevelopment()
                        ? $"<p><strong>Tài khoản đang test:</strong> {WebUtility.HtmlEncode(email)}</p>"
                        : string.Empty)}

                    <p>Mã OTP của bạn là:</p>

                    <div style="font-size:30px;font-weight:700;letter-spacing:8px;margin:24px 0">
                        {otp}
                    </div>

                    <p>Mã OTP có hiệu lực trong <strong>5 phút</strong>.</p>
                    <p>Nếu bạn không thực hiện yêu cầu này, hãy bỏ qua email.</p>
                </div>
                """;

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(username, password)
            };

            await client.SendMailAsync(message);
        }
    }
}