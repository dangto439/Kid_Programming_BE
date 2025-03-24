using AutoMapper;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core;
using KidPrograming.Core.Constants;
using KidPrograming.Entity;
using KidPrograming.Services.Infrastructure;
using KidProgramming.ModelViews.ModelViews.EnrollmentModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace KidPrograming.Services.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Authentication _authentication;
        private readonly FcmService _fcmService;
        private readonly IChapterProgressService _chapterProgressService;

        public EnrollmentService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, Authentication authentication, FcmService fcmService, IChapterProgressService chapterProgressService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _authentication = authentication;
            _fcmService = fcmService;
            _chapterProgressService = chapterProgressService;
        }

        public async Task<PaginatedList<ResponseEnrollmentModel>> CheckStatusCourseByUserId(int index, int pageSize)
        {
            string userId = _authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

            var data = await (from course in _unitOfWork.GetRepository<Course>().Entities.AsNoTracking()
                              join enrollment in _unitOfWork.GetRepository<Enrollment>().Entities.AsNoTracking()
                                  on new { CourseId = course.Id, UserId = userId }
                                  equals new { enrollment.CourseId, enrollment.UserId } into enrollmentGroup
                              from enrollment in enrollmentGroup.DefaultIfEmpty()
                              where !course.DeletedTime.HasValue && (enrollment == null || !enrollment.DeletedTime.HasValue)
                              select new
                              {
                                  CourseId = course.Id,
                                  Title = course.Title,
                                  Enrollment = enrollment,
                                  ThumbnailUrl = course.ThumbnailUrl,
                                  PaymentDate = enrollment != null ? enrollment.Payment.PaymentDate : (DateTimeOffset?)null,
                                  Price = enrollment != null ? enrollment.Course.Price : null
                              }).ToListAsync();

            var responseList = new List<ResponseEnrollmentModel>();

            foreach (var item in data)
            {
                decimal progress = 0;
                if (item.Enrollment != null && !item.Enrollment.DeletedTime.HasValue)
                {
                    progress = await _chapterProgressService.CalculateTotalProgress(item.CourseId, userId);
                }

                responseList.Add(new ResponseEnrollmentModel
                {
                    Id = item.CourseId,
                    Title = item.Title,
                    Status = item.Enrollment != null && !item.Enrollment.DeletedTime.HasValue,
                    Progress = progress,
                    ThumbnailUrl = item.ThumbnailUrl,
                    Price = item.Price,
                    PaymentDate = item.PaymentDate
                    
                });
            }

            return PaginatedList<ResponseEnrollmentModel>.Create(responseList, index, pageSize);
        }

        public async Task CreateEnrollment(string userId, string paymentId, string courseId)
        {
            Enrollment enrollment = new Enrollment
            {
                UserId = userId,
                PaymentId = paymentId,
                CourseId = courseId,
                Status = Core.Constants.Enums.StatusEnrollment.InProgress.ToString()
            };

            Course course = await _unitOfWork.GetRepository<Course>().GetByIdAsync(courseId);

            await _unitOfWork.GetRepository<Enrollment>().InsertAsync(enrollment);

            var chapters = await _unitOfWork.GetRepository<Chapter>()
                .Entities
                .Where(c => c.CourseId == courseId)
                .ToListAsync();

            var chapterProgresses = chapters.Select(chapter => new ChapterProgress
            {
                EnrollmentId = enrollment.Id,
                ChapterId = chapter.Id,
            }).ToList();

            await _unitOfWork.GetRepository<ChapterProgress>().InsertRangeAsync(chapterProgresses);

            await _unitOfWork.SaveAsync();

            User user = await _unitOfWork.GetRepository<User>().GetByIdAsync(userId);

            if (!string.IsNullOrEmpty(user.DeviceToken))
            {
                await _fcmService.SendNotificationAsync(user.DeviceToken, "Register course successfully", $"You have registed {course.Title} sucessfully");
            }
        }

        public async Task<PaginatedList<ResponseEnrollmentModel>> GetEronllmentAsync(int index, int pageSize)
        {
            string userId = _authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

            var data = _unitOfWork.GetRepository<Course>().Entities.AsNoTracking()
                             .GroupJoin(
                                 _unitOfWork.GetRepository<Enrollment>().Entities
                                     .Where(en => en.UserId == userId && !en.DeletedTime.HasValue)
                                     .AsNoTracking(),
                                 course => course.Id,
                                 enrollment => enrollment.CourseId,
                                 (course, enrollmentGroup) => new { course, enrollmentGroup }
                             )
                             .SelectMany(
                                 x => x.enrollmentGroup,
                                 (x, enrollment) => new EnrollmentModel
                                 {
                                     CourseId = x.course.Id,
                                     Title = x.course.Title,
                                     Enrollment = enrollment,
                                     ThumbnailUrl = x.course.ThumbnailUrl,
                                     PaymentDate = enrollment != null ? enrollment.Payment.PaymentDate : (DateTimeOffset?)null,
                                     Price = enrollment != null ? enrollment.Course.Price : null
                                 }
                             )
                            .Where(model => !model.Enrollment.DeletedTime.HasValue);

            var list = await PaginatedList<EnrollmentModel>.CreateAsync(data, index, pageSize);
            var responseList = new List<ResponseEnrollmentModel>();

            foreach (var item in list.Items)
            {
                decimal progress = 0;
                if (item.Enrollment != null && !item.Enrollment.DeletedTime.HasValue)
                {
                    progress = await _chapterProgressService.CalculateTotalProgress(item.CourseId, userId);
                }

                responseList.Add(new ResponseEnrollmentModel
                {
                    Id = item.CourseId,
                    Title = item.Title,
                    Progress = progress,
                    Status = item.Enrollment != null && !item.Enrollment.DeletedTime.HasValue,
                    ThumbnailUrl = item.ThumbnailUrl,
                    Price = item.Price,
                    PaymentDate = item.PaymentDate

                });
            }

            return new PaginatedList<ResponseEnrollmentModel>(responseList, data.Count(), index, pageSize);
        }

        public async Task<PaginatedList<StudentModel>> GetStudentByCourseId(string courseId, string? searchByUserName, int index = 1, int pageSize = 10)
        {
            bool courseExists = await _unitOfWork.GetRepository<Course>().Entities.AnyAsync(c => c.Id == courseId);
            if (!courseExists)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Course not found");
            }

            var query = from enrollment in _unitOfWork.GetRepository<Enrollment>().Entities.AsNoTracking()
                        join user in _unitOfWork.GetRepository<User>().Entities.AsNoTracking()
                            on enrollment.UserId equals user.Id
                        join chapterProgress in _unitOfWork.GetRepository<ChapterProgress>().Entities.AsNoTracking()
                            on new { enrollment.UserId, enrollment.CourseId } equals new { chapterProgress.Enrollment.UserId, CourseId = chapterProgress.Chapter.CourseId } into progressGroup
                        from progress in progressGroup.DefaultIfEmpty()
                        where enrollment.CourseId == courseId && !enrollment.DeletedTime.HasValue
                        group progress by new
                        {
                            enrollment.UserId,
                            user.FullName,
                            user.Email,
                            user.PhoneNumber,
                            user.DateOfBirth,
                            user.AvatarUrl,
                            user.ParentId,
                            user.Role,
                            enrollment.CreatedTime,
                            enrollment.LastUpdatedTime,
                            enrollment.Status
                        }
                        into g
                        select new StudentModel
                        {
                            Id = g.Key.UserId,
                            FullName = g.Key.FullName,
                            Email = g.Key.Email,
                            PhoneNumber = g.Key.PhoneNumber,
                            DateOfBirth = g.Key.DateOfBirth,
                            AvatarUrl = g.Key.AvatarUrl,
                            ParentId = g.Key.ParentId,
                            Role = g.Key.Role.ToString(),
                            StartDate = g.Key.CreatedTime,
                            CompletionDate = g.Key.LastUpdatedTime != g.Key.CreatedTime ? g.Key.LastUpdatedTime : null,
                            StatusOfCourse = g.Key.Status,
                            Progress = (_unitOfWork.GetRepository<Chapter>().Entities.Any(c => c.CourseId == courseId))
                                ? g.Count(p => p.Status == Enums.ProgressStatus.Completed.ToString()) * 100.0m /
                                  (_unitOfWork.GetRepository<Chapter>().Entities.Count(c => c.CourseId == courseId))
    : 0
                        };

            if (!string.IsNullOrWhiteSpace(searchByUserName))
            {
                string searchLower = searchByUserName.ToLower();
                query = query.Where(en => en.FullName != null && en.FullName.ToLower().Contains(searchLower));
            }

            return await PaginatedList<StudentModel>.CreateAsync(query, index, pageSize);
        }

    }
}