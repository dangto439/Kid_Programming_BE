using KidPrograming.Core.Base;
using static KidPrograming.Core.Constants.Enums;

namespace KidPrograming.Entity
{
    public class Enrollment : BaseEntity
    {
        public StatusEnrollment Status { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string CourseId { get; set; }
        public virtual Course Course { get; set; }
        public string PaymentId { get; set; }
        public virtual Payment Payment { get; set; }

        public virtual ICollection<ChapterProgress> ChapterProgresses { get; set; }
    }
}