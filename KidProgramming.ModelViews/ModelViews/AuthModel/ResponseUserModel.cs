namespace KidProgramming.ModelViews.ModelViews.AuthModel
{
    public class ResponseUserModel
    {
        public string? Id { get; set; }

        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTimeOffset? DateOfBirth { get; set; }
        public string? AvatarUrl { get; set; }

        public string? FullName { get; set; }

        public virtual string? ParentId { get; set; }
        public string? Role { get; set; }

        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
    }
}
