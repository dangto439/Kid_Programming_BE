using KidProgramming.ModelViews.ModelViews.DashboardModels;

namespace KidPrograming.Contract.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<List<Revenue>> GetMonthlyRevenueAsync(int year);
        Task<List<TopCourseEnrollment>> GetTopCourseDashboardAsync();
    }
}
