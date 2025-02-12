using KidPrograming.Core;

namespace KidPrograming.Entity
{
    public class Enrollment : BaseEntity
    {
        public DateTime EnrollmentDate { get; set; }
        public virtual User User { get; set; }
        public virtual Course Course { get; set; }
    }
}
