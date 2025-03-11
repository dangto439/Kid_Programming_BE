using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KidPrograming.Core.Constants.Enums;

namespace KidProgramming.ModelViews.ModelViews.LabModels
{
    public class ResponseLabModel
    {
        public string Id { get; set; }
        public string Question { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int Result { get; set; }
        public LabType Type { get; set; }
        public int? LimitedTime { get; set; }
        public string CorrectAnswer { get; set; }
        public string ChapterId { get; set; }
    }
}
