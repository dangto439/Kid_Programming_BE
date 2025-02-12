using KidPrograming.Core;

namespace KidPrograming.Entity
{
    public class Chapter : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }

        public virtual Course Course { get; set; }
    }
}
