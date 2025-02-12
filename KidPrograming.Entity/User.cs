using KidPrograming.Core;

namespace KidPrograming.Entity
{
    public class User : BaseEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // enum Role(Admin, Teacher, Student, Parent)
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string AvatarUrl { get; set; }

        public virtual Guid? ParentId { get; set; }

        public virtual ICollection<User>? Children { get; set; }
        public virtual ICollection<Enrollment>? Enrollments { get; set; }
        public virtual ICollection<Notification>? Notifications { get; set; }
        public virtual ICollection<Course>? Courses { get; set; }
        public virtual ICollection<Payment>? Payments { get; set; }
        public virtual ICollection<Submission>? Submissions { get; set; }
    }
}



