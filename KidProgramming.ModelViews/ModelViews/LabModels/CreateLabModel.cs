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
        public string ToolBox { get; set; }
        public string Solution { get; set; }
        public string ChapterId { get; set; }

        public List<string> Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Title))
                errors.Add("Title is required.");

            if (string.IsNullOrWhiteSpace(Question))
                errors.Add("Question is required.");

            if (string.IsNullOrWhiteSpace(Solution))
                errors.Add("Solution is required.");

            if (string.IsNullOrWhiteSpace(ToolBox))
                errors.Add("Toolbox is required.");

            if (string.IsNullOrWhiteSpace(CorrectAnswer))
                errors.Add("Correct Answer is required.");

            if (Result <= 0 || Result > 10)
                errors.Add("Result is not valid.");

            if (string.IsNullOrWhiteSpace(ChapterId))
                errors.Add("ChapterId is required.");

            if (LimitedTime.HasValue && LimitedTime <= 0)
                errors.Add("LimitedTime must be greater than 0 if specified.");

            return errors;
        }
    }
}