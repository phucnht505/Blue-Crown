namespace BlueCrown.Api.DTOs.Users
{
    public class UserDetailDto
    {
        public Guid Id { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public string? AvatarUrl { get; set; }

        public string? Role { get; set; }

        public string? Status { get; set; }
    }
}
