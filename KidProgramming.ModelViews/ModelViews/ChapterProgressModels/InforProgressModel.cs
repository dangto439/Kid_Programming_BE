namespace KidProgramming.ModelViews.ModelViews.ChapterProgressModels
{
    public class InforProgressModel
    {

       public string CurrentChapterTitle { get; set; }
       public List<LabScore> LabScores { get; set; }
    }
    public class LabScore
    {
        public string LabTitle { get; set; }
        public int Score { get; set; }
    }
}
