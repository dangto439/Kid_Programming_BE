using KidPrograming.Core;
using KidPrograming.Core.Base;
using KidPrograming.Core.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidPrograming.Entity
{
    public class ChapterProgress : BaseEntity
    {
        public string Status { get; set; } = Enums.ProgressStatus.NotStarted.ToString();

        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 100)]
        public decimal Progress { get; set; } = 0;

        public DateTimeOffset? LastAccessed { get; set; } 

        public string EnrollmentId { get; set; }
        public virtual Enrollment Enrollment { get; set; }
        public string ChapterId { get; set; }
        public virtual Chapter Chapter { get; set; }

        public virtual Submission Submission { get; set; }
    }
}