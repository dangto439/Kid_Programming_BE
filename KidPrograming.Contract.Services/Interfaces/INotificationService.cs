using KidProgramming.ModelViews.ModelViews.NotificationModels;

namespace KidPrograming.Contract.Services.Interfaces
{
    public interface INotificationService
    {
        Task Create(BulkNotificationCreateModel model);
    }
}