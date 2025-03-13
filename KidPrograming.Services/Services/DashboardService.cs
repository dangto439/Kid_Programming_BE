using AutoMapper;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.DashboardModels;
using Microsoft.EntityFrameworkCore;

namespace KidPrograming.Services.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DashboardService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<Revenue>> GetMonthlyRevenueAsync(int year)
        {
            List<Revenue> revenues = await _unitOfWork.GetRepository<Payment>().Entities
                .Where(p => !p.DeletedTime.HasValue && p.PaymentDate.Year == year)
                .GroupBy(p => p.PaymentDate.Month)
                .Select(payment => new Revenue
                {
                    Month = payment.Key,
                    TotalRevenue = payment.Sum(p => p.Amount)
                }).OrderBy(p => p.Month).ToListAsync();
            return revenues;
        }

        public async Task<List<TopCourseEnrollment>> GetTopCourseDashboardAsync()
        {
            var topCourses = await _unitOfWork.GetRepository<Enrollment>().Entities
        .Where(e => !e.DeletedTime.HasValue)
        .GroupBy(e => new { e.CourseId, e.Course.Title })
        .Select(group => new TopCourseEnrollment
        {
            CourseId = group.Key.CourseId,
            CourseTitle = group.Key.Title,
            EnrollmentCount = group.Count()
        })
         .OrderByDescending(c => c.EnrollmentCount) 
        .Take(5) 
        .ToListAsync();

            return topCourses;
        }
    }
}
