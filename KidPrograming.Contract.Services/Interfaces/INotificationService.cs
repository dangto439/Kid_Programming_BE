using KidPrograming.Contract.Repositories.PaggingItems;
using KidProgramming.ModelViews.ModelViews.CourseModels;
using KidProgramming.ModelViews.ModelViews.NotificationModels;
using static KidPrograming.Core.Constants.Enums;

namespace KidPrograming.Contract.Services.Interfaces
{
    public interface INotificationService
    {
        Task Create(BulkNotificationCreateModel model);
        Task MarkAsRead(string notificationId);
        Task Delete(string notificationId);
        Task<PaginatedList<ResponseNotificationModel>> GetPage(bool? sortByTitle, string? searchByTitle, bool? isRead, NotificationType? filterByType, int index, int pageSize);
        Task<PaginatedList<ResponseNotificationModel>> GetNotificationsByUserId(int index, int pageSize);
    }
}