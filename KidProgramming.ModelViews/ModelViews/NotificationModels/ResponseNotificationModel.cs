using KidPrograming.Core.Base;
using static KidPrograming.Core.Constants.Enums;

namespace KidProgramming.ModelViews.ModelViews.NotificationModels
{
    public class ResponseNotificationModel : BaseEntity
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public string ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public bool IsRead { get; set; }
    }
}