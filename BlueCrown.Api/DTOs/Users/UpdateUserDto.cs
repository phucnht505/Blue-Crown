namespace BlueCrown.Api.DTOs.Users
{
    public class UpdateUserDto
    {
        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public string? AvatarUrl { get; set; }
    }
}
