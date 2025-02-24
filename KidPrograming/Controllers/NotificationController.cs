using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core.Base;
using KidProgramming.ModelViews.ModelViews.NotificationModels;
using Microsoft.AspNetCore.Mvc;

namespace KidPrograming.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(BulkNotificationCreateModel model)
        {
            await _notificationService.Create(model);
            return Ok(BaseResponse.OkMessageResponse("Created sucessfully"));
        }
    }
}
