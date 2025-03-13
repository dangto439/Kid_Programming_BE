using KidPrograming.Attributes;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core.Base;
using KidProgramming.ModelViews.ModelViews.DashboardModels;
using Microsoft.AspNetCore.Mvc;

namespace KidPrograming.Controllers
{
    [Route("api/dashboards")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        /// <summary>
        /// Get monthly revenue of a year
        /// </summary>
        /// <param name="year">year you want to get data</param>
        /// <returns></returns>
        [HttpGet("revenue")]
        [CacheAtribute(1000)]
        public async Task<IActionResult> GetMonthlyRevenue(int year)
        {
            var results = await _dashboardService.GetMonthlyRevenueAsync(year);
            return Ok(BaseResponseModel<List<Revenue>>.OkDataResponse(results, "Retrieved data sucessfully"));
        }
        /// <summary>
        /// Get top 5 course and number of enrollments 
        /// </summary>
        /// <returns></returns>
        [HttpGet("top-course")]
        [CacheAtribute(1000)]
        public async Task<IActionResult> TopCourseDashboard()
        {
            var results = await _dashboardService.GetTopCourseDashboardAsync();
            return Ok(BaseResponseModel<List<TopCourseEnrollment>>.OkDataResponse(results, "Retrieved data sucessfully"));
        }
    }
}
