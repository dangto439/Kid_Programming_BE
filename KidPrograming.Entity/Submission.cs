using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KidPrograming.Core;

namespace KidPrograming.Entity
{
    public class Submission : BaseEntity
    {
        public int Score { get; set; }
        public DateTimeOffset SubmittedTime { get; set; } = CoreHelper.SystemTimeNow;
        public int TimeSpent { get; set; }

        public Guid UserId { get; set; }
        public required User User { get; set; }
        public Guid LabId { get; set; }
        public required Lab Lab { get; set; }
        public Guid ChapterProgressId { get; set; }
        public required ChapterProgress ChapterProgress { get; set; }
    }
}