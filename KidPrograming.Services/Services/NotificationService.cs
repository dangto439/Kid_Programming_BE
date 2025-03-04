using AutoMapper;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core;
using KidPrograming.Core.Constants;
using KidPrograming.Entity;
using KidPrograming.Services.Infrastructure;
using KidProgramming.ModelViews.ModelViews.NotificationModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace KidPrograming.Services.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Authentication _authentication;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper, Authentication authentication, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _authentication = authentication;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Create(BulkNotificationCreateModel model)
        {
            model.Validate();

            List<string> receiverIds = model.Notifications
                                    .SelectMany(n => n.ReceiverIds)
                                    .Distinct()
                                    .ToList();

            HashSet<string> validUserIds = (await _unitOfWork.GetRepository<User>()
                .Entities
                .Where(u => receiverIds.Contains(u.Id))
                .Select(u => u.Id)
                .ToListAsync())
                .ToHashSet(); 

            List<Notification> notifications = [];

            foreach (CreateNotificationModel item in model.Notifications)
            {
                foreach (string receiverId in item.ReceiverIds)
                {
                    if (!validUserIds.Contains(receiverId)) 
                    {
                        throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT,
                            $"ReceiverId {receiverId} is invalid or does not exist.");
                    }

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

        public async Task Delete(string notificationId)
        {
            Notification notification = await _unitOfWork.GetRepository<Notification>().Entities.FirstOrDefaultAsync(x => x.Id == notificationId && !x.DeletedTime.HasValue) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Notification not found");

            await _unitOfWork.GetRepository<Notification>().DeleteAsync(notification);
            await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<ResponseNotificationModel>> GetNotificationsByUserId(int index, int pageSize)
        {
            string userId = _authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

            IQueryable<ResponseNotificationModel> query = from notification in _unitOfWork.GetRepository<Notification>().Entities
                                                          join user in _unitOfWork.GetRepository<User>().Entities
                                                          on notification.ReceiverId equals user.Id into userGroup
                                                          from user in userGroup.DefaultIfEmpty()
                                                          where (user.Id == userId && !notification.DeletedTime.HasValue)
                                                          orderby notification.CreatedTime descending
                                                          select new ResponseNotificationModel
                                                          {
                                                              Title = notification.Title,
                                                              Message = notification.Message,
                                                              Type = notification.Type,
                                                              ReceiverId = notification.ReceiverId,
                                                              ReceiverName = user.FullName ?? "N/A",
                                                              IsRead = notification.IsRead,
                                                              CreatedTime = notification.CreatedTime
                                                          };

            PaginatedList<ResponseNotificationModel> paginatedCourses = await _unitOfWork.GetRepository<ResponseNotificationModel>().GetPagging(query, index, pageSize);
            return paginatedCourses;
        }

        public async Task<PaginatedList<ResponseNotificationModel>> GetPage(bool? sortByTitle, string? searchByTitle, bool? isRead, Enums.NotificationType? filterByType, int index, int pageSize)
        {
            IQueryable<ResponseNotificationModel> query = from notification in _unitOfWork.GetRepository<Notification>().Entities
                                                          join user in _unitOfWork.GetRepository<User>().Entities
                                                          on notification.ReceiverId equals user.Id into userGroup
                                                          from user in userGroup.DefaultIfEmpty()
                                                          where !notification.DeletedTime.HasValue
                                                          select new ResponseNotificationModel
                                                          {
                                                              Title = notification.Title,
                                                              Message = notification.Message,
                                                              Type = notification.Type,
                                                              ReceiverId = notification.ReceiverId,
                                                              ReceiverName = user.FullName ?? "N/A",
                                                              IsRead = notification.IsRead,
                                                              CreatedTime = notification.CreatedTime
                                                          };
            if (!string.IsNullOrWhiteSpace(searchByTitle))
            {
                searchByTitle = searchByTitle.Trim();
                query = query.Where(x => EF.Functions.Like(x.Title, $"%{searchByTitle}%"));
            }

            if (filterByType.HasValue)
            {
                query = query.Where(x => x.Type == filterByType);
            }

            if (sortByTitle.HasValue)
            {
                query = sortByTitle.HasValue ? query.OrderBy(x => x.Title) : query.OrderByDescending(x => x.Title);
            }

            if (isRead.HasValue)
            {
                query = query.Where(n => n.IsRead == isRead.Value);
            }

            PaginatedList<ResponseNotificationModel> paginatedCourses = await _unitOfWork.GetRepository<ResponseNotificationModel>().GetPagging(query, index, pageSize);
            return paginatedCourses;
        }

        public async Task MarkAsRead(string notificationId)
        {
            Notification notification = await _unitOfWork.GetRepository<Notification>().Entities.FirstOrDefaultAsync(x => x.Id == notificationId && !x.DeletedTime.HasValue) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Notification not found");

            notification.IsRead = true;
            await _unitOfWork.SaveAsync();
        }
    }
}