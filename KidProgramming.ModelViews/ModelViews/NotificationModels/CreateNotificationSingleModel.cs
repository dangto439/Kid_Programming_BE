using KidPrograming.Core;
using Microsoft.AspNetCore.Http;
using static KidPrograming.Core.Constants.Enums;

namespace KidProgramming.ModelViews.ModelViews.NotificationModels
{
    public class CreateNotificationSingleModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string? CourseId { get; set; }
        public NotificationType Type { get; set; }
        public string ReceiverId { get; set; }

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

            if (string.IsNullOrWhiteSpace(ReceiverId))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Please enter a receiverId for the notification");
            }

            if (!Enum.IsDefined(typeof(NotificationType), Type))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Invalid notification type specified");
            }
        }
    }
}