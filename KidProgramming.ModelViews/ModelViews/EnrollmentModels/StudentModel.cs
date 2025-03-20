using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 100)]
        public decimal Progress { get; set; } = 0;
        public string StatusOfCourse { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? CompletionDate { get; set; }
    }
}
