using AutoMapper;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.CourseModels;
using KidProgramming.ModelViews.ModelViews.EnrollmentModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KidPrograming.Services.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EnrollmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
            await _unitOfWork.GetRepository<Enrollment>().InsertAsync(enrollment);
            await _unitOfWork.GetRepository<Enrollment>().SaveAsync();
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
