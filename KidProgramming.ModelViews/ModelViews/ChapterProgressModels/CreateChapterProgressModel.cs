using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KidPrograming.Core.Constants.Enums;

namespace KidProgramming.ModelViews.ModelViews.ChapterProgressModels
{
    public class CreateChapterProgressModel
    {
        [Required]
        public CompletionStatus Status { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Progress must be between 0 and 100.")]
        public decimal Progress { get; set; }

        public DateTimeOffset? LastAccessed { get; set; }

        [Required(ErrorMessage = "EnrollmentId is required.")]
        public string EnrollmentId { get; set; }

        [Required(ErrorMessage = "ChapterId is required.")]
        public string ChapterId { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(EnrollmentId))
            {
                throw new ValidationException("EnrollmentId cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(ChapterId))
            {
                throw new ValidationException("ChapterId cannot be empty.");
            }

            if (Progress < 0 || Progress > 100)
            {
                throw new ValidationException("Progress must be between 0 and 100.");
            }
        }
    }
}
