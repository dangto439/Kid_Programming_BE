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

        public EnrollmentService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, Authentication authentication, FcmService fcmService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _authentication = authentication;
            _fcmService = fcmService;
        }

        public async Task<PaginatedList<ResponseEnrollmentModel>> CheckStatusCourseByUserId(int index, int pageSize)
        {
            string userId = _authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

            IQueryable<ResponseEnrollmentModel> query =
                from course in _unitOfWork.GetRepository<Course>().Entities
                join enrollment in _unitOfWork.GetRepository<Enrollment>().Entities
                    on new { CourseId = course.Id, UserId = userId }
                    equals new { enrollment.CourseId, enrollment.UserId } into enrollmentGroup
                from enrollment in enrollmentGroup.DefaultIfEmpty()
                where !course.DeletedTime.HasValue
                select new ResponseEnrollmentModel
                {
                    Id = course.Id,
                    Title = course.Title,
                    Status = enrollment != null && !enrollment.DeletedTime.HasValue
                };

            return await PaginatedList<ResponseEnrollmentModel>.CreateAsync(query, index, pageSize);
        }


        public async Task CreateEnrollment(string userId, string paymentId, string courseId)
        {
            Enrollment enrollment = new Enrollment
            {
                UserId = userId,
                PaymentId = paymentId,
                CourseId = courseId,
                Status = Core.Constants.Enums.StatusEnrollment.InProgress
            };
            Course course = await _unitOfWork.GetRepository<Course>().GetByIdAsync(courseId);
            await _unitOfWork.GetRepository<Enrollment>().InsertAsync(enrollment);
            await _unitOfWork.GetRepository<Enrollment>().SaveAsync();
            User user = await _unitOfWork.GetRepository<User>().GetByIdAsync(userId);
            if (!string.IsNullOrEmpty(user.DeviceToken))
            {
                await _fcmService.SendNotificationAsync(user.DeviceToken, "Đăng ký khóa học thành công", $"Bạn đã đăng ký khóa học {course.Title} thành công");
            }
        }

        public async Task<PaginatedList<StudentModel>> GetStudentByCourseId(string courseId, string searchByUserName, int index = 1, int pageSize = 10)
        {
            IQueryable<StudentModel> enrollments = _unitOfWork.GetRepository<Enrollment>().Entities
                                    .Where(enrollment => enrollment.CourseId == courseId && !enrollment.DeletedTime.HasValue)
                                    .Include(enrollment => enrollment.User)
                                    .Where(en => en.User.FullName!.ToLower().Contains(searchByUserName.ToLower()))
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

            return await PaginatedList<StudentModel>.CreateAsync(enrollments, index, pageSize);
        }
    }
}