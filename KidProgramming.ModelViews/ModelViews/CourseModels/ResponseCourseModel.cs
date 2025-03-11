using KidPrograming.Core.Base;

namespace KidProgramming.ModelViews.ModelViews.CourseModels
{
    public class ResponseCourseModel : BaseEntity
    {
        public string Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Subject { get; set; }
        public string? ThumbnailUrl { get; set; }
        public decimal? Price { get; set; }
        public string? Status { get; set; }
        public int? NumOfChapter { get; set; }

        // public string TeacherId { get; set; }
        public string? TeacherName { get; set; }
    }
}