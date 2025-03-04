using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidProgramming.ModelViews.ModelViews.SubmissionModels
{
    public class UpdateSubmissionModel
    {
        public int? Score { get; set; }
        public int? TimeSpent { get; set; }

        public void Validate()
        {
            if (Score.HasValue && (Score < 0 || Score > 100))
                throw new ValidationException("Score must be between 0 and 100.");

            if (TimeSpent.HasValue && TimeSpent < 0)
                throw new ValidationException("Time spent must be a positive number.");
        }
    }
}
