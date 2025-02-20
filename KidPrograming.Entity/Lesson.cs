using KidPrograming.Core;

namespace KidPrograming.Entity
{
    public class Lesson : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? MaterialUrl { get; set; }
        public int Order { get; set; }

        public string ChapterId { get; set; }
        public virtual Chapter Chapter { get; set; }

        public ICollection<Lab>? Labs { get; set; }
    }
}