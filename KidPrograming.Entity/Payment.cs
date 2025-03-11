using KidPrograming.Core;
using KidPrograming.Core.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidPrograming.Entity
{
    public class Payment : BaseEntity
    {
        [Column(TypeName = "decimal(19,0)")]
        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = "VNPAY";
        public DateTimeOffset PaymentDate { get; set; } = CoreHelper.SystemTimeNow;
        public string Status { get; set; }
    }
}