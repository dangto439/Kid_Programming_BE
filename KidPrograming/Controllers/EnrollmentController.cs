using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core.Base;
using KidProgramming.ModelViews.ModelViews.EnrollmentModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace KidPrograming.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        /// <summary>
        /// Get students has enrolled in course by courseId 
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="searchByName"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetStudentByCourseId([Required] string courseId, string? searchByName, int index = 1, int pageSize = 10)
        {
            var result = await _enrollmentService.GetStudentByCourseId(courseId, searchByName, index, pageSize);
            return Ok(BaseResponseModel<string>.OkDataResponse(result, "Retrieved student list for course successfully"));
        }

        /// <summary>
        /// Get list course attache status of online-user (if status true = buyed - false = not purchased)
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCourseForUser(int index = 1, int pageSize = 10)
        {
            var result = await _enrollmentService.CheckStatusCourseByUserId(index, pageSize);
            return Ok(BaseResponseModel<PaginatedList<ResponseEnrollmentModel>>.OkDataResponse(result, "Retrieved course status list for user successfully"));
        }

        [HttpGet("enrolled-courses")]
        public async Task<IActionResult> GetEnrollment(string? courseId ,int index = 1, int pageSize = 10)
        {
            var result = await _enrollmentService.GetEronllmentAsync(courseId,index, pageSize);
            return Ok(BaseResponseModel<PaginatedList<ResponseEnrollmentModel>>.OkDataResponse(result, "Retrieved course status list for user successfully"));
        }
    }
}