namespace KidProgramming.ModelViews.ModelViews.ChapterProgressModels
{
    public class InforProgressModel
    {

       public string CurrentChapterTiltel { get; set; }
       public List<LabScore> LabScores { get; set; }
    }
public class LabScore
{
    public string LabTile { get; set; }
    public int Score { get; set; }
}
}
