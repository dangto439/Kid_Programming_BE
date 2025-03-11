using KidPrograming.Core.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidPrograming.Entity
{
    public class Notification : BaseEntity
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public bool IsRead { get; set; } = false;

        public string ReceiverId { get; set; }

        [ForeignKey(nameof(ReceiverId))]
        public virtual User User { get; set; }
    }
}