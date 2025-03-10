namespace KidProgramming.ModelViews.ModelViews.AuthModel
{
    public class UserResponse
    {
        public string? Id { get; set; }

        public string? Email { get; set; }

        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Role { get; set; }
        public string? AvatarUrl { get; set; }

        public DateTimeOffset CreatedTime { get; set; }
    }
}
