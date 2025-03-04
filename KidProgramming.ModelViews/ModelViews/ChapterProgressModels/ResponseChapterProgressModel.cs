using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KidPrograming.Core.Constants.Enums;

namespace KidProgramming.ModelViews.ModelViews.ChapterProgressModels
{
    public class ResponseChapterProgressModel
    {
        public string Id { get; set; }
        public CompletionStatus Status { get; set; }
        public decimal Progress { get; set; }
        public DateTimeOffset? LastAccessed { get; set; }
        public string EnrollmentId { get; set; }
        public string ChapterId { get; set; }
    }
}
