using KidPrograming.Core;

namespace KidPrograming.Entity
{
    public class Notification : BaseEntity
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
