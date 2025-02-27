using KidPrograming.Core.Base;

namespace KidProgramming.ModelViews.ModelViews.ChapterModels
{
    public class ResponseChapterModel : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
    }
}