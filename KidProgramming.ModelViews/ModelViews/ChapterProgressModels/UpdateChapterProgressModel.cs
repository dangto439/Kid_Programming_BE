using System.ComponentModel.DataAnnotations;
using static KidPrograming.Core.Constants.Enums;

namespace KidProgramming.ModelViews.ModelViews.ChapterProgressModels
{
    public class UpdateChapterProgressModel
    {
        public ProgressStatus? Status { get; set; }

        [Range(0, 100, ErrorMessage = "Progress must be between 0 and 100.")]
        public decimal? Progress { get; set; }

        public DateTimeOffset? LastAccessed { get; set; }

        public void Validate()
        {
            if (Progress.HasValue && (Progress < 0 || Progress > 100))
            {
                throw new ValidationException("Progress must be between 0 and 100.");
            }
        }
    }
}