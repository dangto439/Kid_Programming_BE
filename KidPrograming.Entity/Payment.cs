using KidPrograming.Core;
using KidPrograming.Core.Base;
using System.ComponentModel.DataAnnotations.Schema;
using static KidPrograming.Core.Constants.Enums;

namespace KidPrograming.Entity
{
    public class Payment : BaseEntity
    {
        [Column(TypeName = "decimal(19,0)")]
        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = "VNPAY";
        public DateTimeOffset PaymentDate { get; set; } = CoreHelper.SystemTimeNow;
        public StatusPayment Status { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string CourseId { get; set; }
        public virtual Course Course { get; set; }
    }
}