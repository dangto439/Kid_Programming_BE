using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KidPrograming.Core.Constants.Enums;

namespace KidProgramming.ModelViews.ModelViews.LabModels
{
    public class UpdateLabModel
    {
        public string? Question { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? Result { get; set; }
        public LabType? Type { get; set; }
        public int? LimitedTime { get; set; }
        public string? CorrectAnswer { get; set; }

        public List<string> Validate()
        {
            var errors = new List<string>();

            if (Result.HasValue && Result <= 0)
                errors.Add("Result must be greater than 0 if specified.");

            if (LimitedTime.HasValue && LimitedTime <= 0)
                errors.Add("LimitedTime must be greater than 0 if specified.");

            return errors;
        }
    }
}
