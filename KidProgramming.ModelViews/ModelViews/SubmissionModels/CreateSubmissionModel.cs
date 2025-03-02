using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidProgramming.ModelViews.ModelViews.SubmissionModels
{
    public class CreateSubmissionModel
    {
        [Required]
        public int Score { get; set; }

        [Required]
        public int TimeSpent { get; set; }

        //[Required]
        //public string UserId { get; set; }

        [Required]
        public string LabId { get; set; }

        [Required]
        public string ChapterProgressId { get; set; }

        public void Validate()
        {
            if (Score < 0 || Score > 100)
                throw new ValidationException("Score must be between 0 and 100.");

            if (TimeSpent < 0)
                throw new ValidationException("Time spent must be a positive number.");

            //if (string.IsNullOrWhiteSpace(UserId))
            //    throw new ValidationException("UserId is required.");

            if (string.IsNullOrWhiteSpace(LabId))
                throw new ValidationException("LabId is required.");

            if (string.IsNullOrWhiteSpace(ChapterProgressId))
                throw new ValidationException("ChapterProgressId is required.");
        }
    }
}
