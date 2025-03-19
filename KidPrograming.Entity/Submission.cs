using KidPrograming.Core;
using KidPrograming.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace KidPrograming.Entity
{
    public class Submission : BaseEntity
    {
        [Range(1, 10, ErrorMessage = "Score must be between 1 to 10")]
        public int Score { get; set; } // 1 - 10 only
        public DateTimeOffset SubmittedTime { get; set; } = CoreHelper.SystemTimeNow;
        public int TimeSpent { get; set; }
        public string? CreativeAnswer { get; set; } = string.Empty;
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string LabId { get; set; }
        public virtual Lab Lab { get; set; }
        public string ChapterProgressId { get; set; }
        public virtual ChapterProgress ChapterProgress { get; set; }
    }
}