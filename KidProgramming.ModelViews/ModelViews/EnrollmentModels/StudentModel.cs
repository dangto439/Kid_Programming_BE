namespace KidProgramming.ModelViews.ModelViews.EnrollmentModels
{
    public class StudentModel
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTimeOffset? DateOfBirth { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Role { get; set; }
        public virtual string? ParentId { get; set; }
    }
}
