using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KidPrograming.Core
{
    public class Enums
    {
        public enum LabType
        {
            Quiz,
            Block
        }

        public enum StatusEnrollment
        {
            InProgress,
            Completed
        }

        public enum StatusPayment
        {
            Pending,
            Success,
            Failed,
            Canceled
        }

        public enum CompletionStatus
        {
            InProgress,
            Completed
        }
    }
}