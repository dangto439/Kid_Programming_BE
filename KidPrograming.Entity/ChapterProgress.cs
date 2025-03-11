using KidPrograming.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidPrograming.Entity
{
    public class ChapterProgress : BaseEntity
    {
        public string Status { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 100)]
        public decimal Progress { get; set; }

        public DateTimeOffset? LastAccessed { get; set; }

        public string EnrollmentId { get; set; }
        public virtual Enrollment Enrollment { get; set; }
        public string ChapterId { get; set; }
        public virtual Chapter Chapter { get; set; }

        public ICollection<Submission>? Submissions { get; set; }
    }
}