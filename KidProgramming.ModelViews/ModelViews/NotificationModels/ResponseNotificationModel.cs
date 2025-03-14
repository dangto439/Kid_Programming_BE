﻿using KidPrograming.Core.Base;

namespace KidProgramming.ModelViews.ModelViews.NotificationModels
{
    public class ResponseNotificationModel : BaseEntity
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public string ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public bool IsRead { get; set; }
    }
}