using KidPrograming.Core;
using System.ComponentModel.DataAnnotations.Schema;
using static KidPrograming.Core.Enums;


namespace KidPrograming.Entity
{
    public class User : BaseEntity
    {
        public string? FullName { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? AvatarUrl { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public  Role  Role { get; set; }
        public virtual string? ParentId { get; set; }
        public virtual User Parent { get; set; }
        public virtual ICollection<User>? Children { get; set; }
        public virtual ICollection<Enrollment>? Enrollments { get; set; }
        public virtual ICollection<Notification>? Notifications { get; set; }
        public virtual ICollection<Course>? Courses { get; set; }
        public virtual ICollection<Payment>? Payments { get; set; }
        public virtual ICollection<Submission>? Submissions { get; set; }
    }
}


