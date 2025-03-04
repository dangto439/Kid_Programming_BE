using KidPrograming.Core.Base;
using System.ComponentModel.DataAnnotations.Schema;
using static KidPrograming.Core.Constants.Enums;

namespace KidPrograming.Entity
{
    public class Course : BaseEntity
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Subject { get; set; }
        public string? ThumbnailUrl { get; set; }

        [Column(TypeName = "decimal(19,0)")]
        public decimal? Price { get; set; }

        public CourseStatus Status { get; set; }

        public string? TeacherId { get; set; }
        public virtual User? Teacher { get; set; }
        public virtual ICollection<Chapter>? Chapters { get; set; }
        public virtual ICollection<Enrollment>? Enrollments { get; set; }
    }
}