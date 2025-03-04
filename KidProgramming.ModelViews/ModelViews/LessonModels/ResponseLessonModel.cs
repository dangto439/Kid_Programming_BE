using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidProgramming.ModelViews.ModelViews.LessonModels
{
    public class ResponseLessonModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? MaterialUrl { get; set; }
        public int Order { get; set; }
        public string ChapterId { get; set; }
    }
}
