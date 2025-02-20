using KidPrograming.Core.Base;

namespace KidPrograming.Entity
{
    public class Chapter : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }

        public string CourseId { get; set; }
        public virtual Course Course { get; set; }

        public virtual ICollection<Lesson> Lessons { get; set; }
        public virtual ICollection<ChapterProgress> ChapterProgresses { get; set; }
        
    }
}
