using KidPrograming.Core.Base;
using static KidPrograming.Core.Constants.Enums;

namespace KidProgramming.ModelViews.ModelViews.CourseModels
{
    public class ResponseCourseModel : BaseEntity
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Subject { get; set; }
        public decimal? Price { get; set; }
        public CourseStatus? Status { get; set; }
        // public string TeacherId { get; set; }
        public string? TeacherName { get; set; }
    }
}