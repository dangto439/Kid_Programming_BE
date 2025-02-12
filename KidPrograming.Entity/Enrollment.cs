using KidPrograming.Core;
using static KidPrograming.Core.Enums;

namespace KidPrograming.Entity
{
    public class Enrollment : BaseEntity
    {
        public DateTime EnrollmentDate { get; set; }
        public StatusEnrollment Status { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }
        public Guid CourseId { get; set; }
        public virtual Course Course { get; set; }
        
        public virtual ICollection<ChapterProgress> ChapterProgresses { get; set; }
    }
}
