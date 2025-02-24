using KidPrograming.Core;
using Microsoft.AspNetCore.Http;

namespace KidProgramming.ModelViews.ModelViews.NotificationModels
{
    public class BulkNotificationCreateModel
    {
        public List<CreateNotificationModel> Notifications { get; set; }

        public void Validate()
        {
            if (Notifications == null || Notifications.Count == 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "At least one notification is required.");
            }

            foreach (CreateNotificationModel notification in Notifications)
            {
                notification.Validate();
            }
        }
    }
}