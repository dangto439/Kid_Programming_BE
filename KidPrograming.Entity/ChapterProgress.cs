using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using KidPrograming.Core;
using KidPrograming.Core.Base;
using static KidPrograming.Core.Constants.Enums;

namespace KidPrograming.Entity
{
    public class ChapterProgress : BaseEntity
    {
        public CompletionStatus Status { get; set; }
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