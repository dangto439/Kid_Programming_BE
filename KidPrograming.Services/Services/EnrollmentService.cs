using AutoMapper;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Entity;
using KidPrograming.Services.Infrastructure;
using KidProgramming.ModelViews.ModelViews.EnrollmentModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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

        //public async Task<PaginatedList<ResponseEnrollmentModel>> CheckStatusCourseByUserId(int index, int pageSize)
        //{
        //    string userId = _authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

        //    IQueryable<ResponseEnrollmentModel> query =
        //        from course in _unitOfWork.GetRepository<Course>().Entities
        //        join enrollment in _unitOfWork.GetRepository<Enrollment>().Entities
        //            on new { CourseId = course.Id, UserId = userId }
        //            equals new { enrollment.CourseId, enrollment.UserId } into enrollmentGroup
        //        from enrollment in enrollmentGroup.DefaultIfEmpty()
        //        where !course.DeletedTime.HasValue
        //        select new ResponseEnrollmentModel
        //        {
        //            Id = course.Id,
        //            Title = course.Title,
        //            Progress = _chapterProgressService.CalculateTotalProgress(course.Id),
        //            Status = enrollment != null && !enrollment.DeletedTime.HasValue && !course.DeletedTime.HasValue
        //        };

        //    return await PaginatedList<ResponseEnrollmentModel>.CreateAsync(query, index, pageSize);
        //}

        public async Task<PaginatedList<ResponseEnrollmentModel>> CheckStatusCourseByUserId(int index, int pageSize)
        {
            string userId = _authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

            var enrollments = await _unitOfWork.GetRepository<Enrollment>()
                .Entities.AsNoTracking()
                .Where(e => e.UserId == userId && !e.DeletedTime.HasValue)
                .ToListAsync();

            var courses = await _unitOfWork.GetRepository<Course>()
                .Entities.AsNoTracking()
                .Where(c => !c.DeletedTime.HasValue)
                .ToListAsync();

            var data = (from course in courses
                        join enrollment in enrollments on course.Id equals enrollment.CourseId into enrollmentGroup
                        from enrollment in enrollmentGroup.DefaultIfEmpty()
                        select new
                        {
                            Course = course,
                            //CourseId = course.Id,
                            Title = course.Title,
                            Enrollment = enrollment,
                        }).ToList();

            var responseList = new List<ResponseEnrollmentModel>();

            foreach (var item in data)
            {
                decimal progress = item.Enrollment != null ? await _chapterProgressService.CalculateTotalProgress(item.Course.Id, item.Enrollment.UserId) : 0;

                var enrollment = item.Enrollment;
                bool hasEnrollment = enrollment != null;

                responseList.Add(new ResponseEnrollmentModel
                {
                    Id = item.Course.Id,
                    Title = item.Title,
                    Progress = progress,
                    ThumbnailUrl = item.Course.ThumbnailUrl != null ? item.Course.ThumbnailUrl : string.Empty,
                    StatusOfCourse = hasEnrollment ? enrollment.Status : "Not Enrolled",
                    startDate = hasEnrollment ? enrollment.CreatedTime : null,
                    completionDate = (hasEnrollment && enrollment.LastUpdatedTime != enrollment.CreatedTime)
                        ? enrollment.LastUpdatedTime
                        : null,
                    Status = hasEnrollment && !enrollment.DeletedTime.HasValue
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

        public async Task<PaginatedList<StudentModel>> GetStudentByCourseId(string courseId, string? searchByUserName, int index = 1, int pageSize = 10)
        {
            IQueryable<StudentModel> enrollments = _unitOfWork.GetRepository<Enrollment>().Entities
                                    .Where(enrollment => enrollment.CourseId == courseId && !enrollment.DeletedTime.HasValue)
                                    .Include(enrollment => enrollment.User)
                                    .Select(user => new StudentModel
                                    {
                                        Id = user.User.Id,
                                        FullName = user.User.FullName,
                                        Email = user.User.Email,
                                        PhoneNumber = user.User.PhoneNumber,
                                        DateOfBirth = user.User.DateOfBirth,
                                        AvatarUrl = user.User.AvatarUrl,
                                        ParentId = user.User.ParentId,
                                        Role = user.User.Role.ToString()
                                    });
            if (!string.IsNullOrWhiteSpace(searchByUserName))
            {
                enrollments = enrollments.Where(en => en.FullName!.ToLower().Contains(searchByUserName.ToLower()));
            }
            return await PaginatedList<StudentModel>.CreateAsync(enrollments, index, pageSize);
        }
    }
}