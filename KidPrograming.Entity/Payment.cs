using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using KidPrograming.Core;
using static KidPrograming.Core.Enums;

namespace KidPrograming.Entity
{
    public class Payment : BaseEntity
    {
        [Column(TypeName = "decimal(19,0)")]
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "VNPAY";
        public DateTimeOffset PaymentDate { get; set; } = CoreHelper.SystemTimeNow;
        public StatusPayment Status { get; set; }

        public Guid UserId { get; set; }
        public required User User { get; set; }
        public Guid CourseId { get; set; }
        public required Course Course { get; set; }
    }
}