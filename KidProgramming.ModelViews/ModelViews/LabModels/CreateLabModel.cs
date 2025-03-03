using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KidPrograming.Core.Constants.Enums;

namespace KidProgramming.ModelViews.ModelViews.LabModels
{
    public class CreateLabModel
    {
        public string Question { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Result { get; set; }
        public LabType Type { get; set; }
        public int? LimitedTime { get; set; }
        public string CorrectAnswer { get; set; } = string.Empty;
        public string LessonId { get; set; }

        public List<string> Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Title))
                errors.Add("Title is required.");

            if (string.IsNullOrWhiteSpace(Question))
                errors.Add("Question is required.");

            if (string.IsNullOrWhiteSpace(CorrectAnswer))
                errors.Add("Correct Answer is required.");

            if (Result <= 0)
                errors.Add("Result must be greater than 0.");

            if (string.IsNullOrWhiteSpace(LessonId))
                errors.Add("LessonId is required.");

            if (LimitedTime.HasValue && LimitedTime <= 0)
                errors.Add("LimitedTime must be greater than 0 if specified.");

            return errors;
        }
    }
}
