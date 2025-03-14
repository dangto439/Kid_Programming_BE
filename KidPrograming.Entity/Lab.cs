using KidPrograming.Core.Base;

namespace KidPrograming.Entity
{
    public class Lab : BaseEntity
    {
        public string Question { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Result { get; set; } // điểm tối đa có thể đạt được khi làm bài thực hành này
        public string Type { get; set; }
        public int? LimitedTime { get; set; } // có thể có bài lab k giới hạn time => null
        public string CorrectAnswer { get; set; } = string.Empty;

        public string ChapterId { get; set; }
        public virtual Chapter Chapter { get; set; }
    }
}