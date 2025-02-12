using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using KidPrograming.Core;
using static KidPrograming.Core.Enums;

namespace KidPrograming.Entity
{
    public class ChapterProgress : BaseEntity
    {
        public CompletionStatus Status { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 100)]
        public decimal Progress { get; set; }
        public DateTimeOffset? LastAccessed { get; set; }

        public Guid EnrollmentId { get; set; }
        public required Enrollment Enrollment { get; set; }
        public Guid ChapterId { get; set; }
        public required Chapter Chapter { get; set; }

        public ICollection<Submission>? Submissions { get; set; }
    }
}