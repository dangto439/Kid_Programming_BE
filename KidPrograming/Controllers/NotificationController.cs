using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core.Base;
using KidProgramming.ModelViews.ModelViews.NotificationModels;
using Microsoft.AspNetCore.Mvc;
using static KidPrograming.Core.Constants.Enums;

namespace KidPrograming.Controllers
{
    [ApiController]
    [Route("/api/notification")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(bool? sortByTitle, string? searchByTitle, bool? isRead, NotificationType? filterByType, int index = 1, int pageSize = 10)
        {
            PaginatedList<ResponseNotificationModel> result = await _notificationService.GetPage(sortByTitle, searchByTitle, isRead, filterByType, index, pageSize);
            return Ok(BaseResponseModel<PaginatedList<ResponseNotificationModel>>.OkDataResponse(result, "Retrieve notification list successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create(BulkNotificationCreateModel model)
        {
            await _notificationService.Create(model);
            return Ok(BaseResponse.OkMessageResponse("Created sucessfully"));
        }

        [HttpPatch("{notificationId}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead(string notificationId)
        {
            await _notificationService.MarkAsRead(notificationId);
            return Ok(BaseResponse.OkMessageResponse("Notification marked as read successfully"));
        }

        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> Delete(string notificationId)
        {
            await _notificationService.Delete(notificationId);
            return Ok(BaseResponse.OkMessageResponse("Deleted sucessfully"));
        }
    }
}