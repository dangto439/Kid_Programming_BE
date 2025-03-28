using KidPrograming.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace KidPrograming.Entity
{
    public class Lab : BaseEntity
    {
        public string Question { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Range(1, 10, ErrorMessage = "Result must be between 1 to 10")]
        public int Result { get; set; }
        public string Type { get; set; }
        public int? LimitedTime { get; set; } 
        public string CorrectAnswer { get; set; } = string.Empty;
        public string ToolBox { get; set; }
        public string Solution { get; set; }
        public string ChapterId { get; set; }
        public virtual Chapter Chapter { get; set; }
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();


    }
}