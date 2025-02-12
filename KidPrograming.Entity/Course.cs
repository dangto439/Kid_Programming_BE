using KidPrograming.Core;

namespace KidPrograming.Entity
{
    public class Course : BaseEntity
    {
        
        public string Title { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        public string ThumbnailUrl { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }

        public virtual User Teacher { get; set; }
        public virtual ICollection<Chapter> Chapters { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}
