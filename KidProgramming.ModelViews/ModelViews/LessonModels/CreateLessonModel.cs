using System.ComponentModel.DataAnnotations;

namespace KidProgramming.ModelViews.ModelViews.LessonModels
{
    public class CreateLessonModel
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Url]
        public string? MaterialUrl { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Order must be greater than 0")]
        public int Order { get; set; }

        [Required]
        public string ChapterId { get; set; }

        public void Validate()
        {
            Validator.ValidateObject(this, new ValidationContext(this), true);
        }
    }
}