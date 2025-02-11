using KidPrograming.Core;

namespace KidPrograming.Entity
{
    public class User : BaseEntity
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; }
        public string? AvatarUrl { get; set; }

    }
}



