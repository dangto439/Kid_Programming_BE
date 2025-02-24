using AutoMapper;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.NotificationModels;

namespace KidPrograming.Services.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task Create(BulkNotificationCreateModel model)
        {
            model.Validate();

            List<Notification> notifications = [];

            foreach (CreateNotificationModel item in model.Notifications)
            {
                foreach (string receiverId in item.ReceiverIds)
                {
                    notifications.Add(new Notification
                    {
                        Title = item.Title,
                        Message = item.Message,
                        ReceiverId = receiverId,
                        Type = item.Type,
                        IsRead = false,
                        CreatedTime = CoreHelper.SystemTimeNow
                    });
                }
            }

            await _unitOfWork.GetRepository<Notification>().InsertRangeAsync(notifications);
            await _unitOfWork.SaveAsync();
        }
    }
}