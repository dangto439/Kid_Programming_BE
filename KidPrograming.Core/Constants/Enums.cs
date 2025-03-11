namespace KidPrograming.Core.Constants
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

        public enum Role
        {
            Admin,
            Customer,
            Parent,
            Teacher
        }

        public enum NotificationType
        {
            CourseUpdate,
            Assignment,
            Payment,
            System
        }

        public enum LessonType
        {
            Video,
            Quiz,
            Article
        }
    }
}