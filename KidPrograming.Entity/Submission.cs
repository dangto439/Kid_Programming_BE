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

        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string LabId { get; set; }
        public virtual Lab Lab { get; set; }
        public string ChapterProgressId { get; set; }
        public virtual ChapterProgress ChapterProgress { get; set; }
    }
}