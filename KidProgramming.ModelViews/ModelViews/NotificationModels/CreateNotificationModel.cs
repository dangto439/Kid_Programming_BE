using KidPrograming.Core;
using Microsoft.AspNetCore.Http;
using static KidPrograming.Core.Constants.Enums;

namespace KidProgramming.ModelViews.ModelViews.NotificationModels
{
    public class CreateNotificationModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public List<string> ReceiverIds { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Please enter a title for the notification");
            }

            if (string.IsNullOrWhiteSpace(Message))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Please enter a message for the notification");
            }

            if (ReceiverIds == null || ReceiverIds.Count == 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Please specify at least one receiver for the notification");
            }

            if (!Enum.IsDefined(typeof(NotificationType), Type))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Invalid notification type specified");
            }
        }
    }
}