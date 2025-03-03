using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidProgramming.ModelViews.ModelViews.SubmissionModels
{
    public class ResponseSubmissionModel
    {
        public string Id { get; set; }
        public int Score { get; set; }
        public DateTimeOffset SubmittedTime { get; set; }
        public int TimeSpent { get; set; }
        public string UserId { get; set; }
        public string LabId { get; set; }
        public string ChapterProgressId { get; set; }
    }
}
