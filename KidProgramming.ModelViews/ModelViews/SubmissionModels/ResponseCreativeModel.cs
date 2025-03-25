using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidProgramming.ModelViews.ModelViews.SubmissionModels
{
    public class ResponseCreativeModel
    {
        public string SubmissionId { get; set; }
        public DateTimeOffset SubmittedTime { get; set; }
        public string DescriptionLab { get; set; }
        public int Score { get; set; }
        public string AnswerCheck { get; set; }

    }
}
