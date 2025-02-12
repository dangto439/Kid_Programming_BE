using KidPrograming.Core;

namespace KidPrograming.Entity
{
    public class User : BaseEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string AvatarUrl { get; set; }

        public Guid? ParentId { get; set; }  // Khóa ngoại tự tham chiếu

        // Navigation property để reference đến User cha (Parent)
        public virtual User? Parent { get; set; }

        // Navigation property để reference đến danh sách User con (Children)
        public virtual ICollection<User>? Children { get; set; }

        public virtual ICollection<Enrollment>? Enrollments { get; set; }
        public virtual ICollection<Notification>? Notifications { get; set; }
    }
}



