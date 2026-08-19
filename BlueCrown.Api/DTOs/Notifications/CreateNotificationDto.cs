namespace BlueCrown.Api.DTOs.Notifications
{
    public class CreateNotificationDto
    {
        public Guid UserId { get; set; }
        public string? Type { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
    }
}