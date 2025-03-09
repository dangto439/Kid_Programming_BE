using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core.Base;
using KidProgramming.ModelViews.ModelViews.EnrollmentModels;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetStudentByCourseId(string courseId, string? searchByName, int index = 1, int pageSize = 10)
        {
            var result = await _enrollmentService.GetStudentByCourseId(courseId, searchByName, index, pageSize);
            return Ok(BaseResponseModel<string>.OkDataResponse(result, "Login success"));
        }

        [HttpGet]
        public async Task<IActionResult> GetCourseForUser(int index = 1, int pageSize = 10)
        {
            PaginatedList<ResponseEnrollmentModel> result = await _enrollmentService.CheckStatusCourseByUserId(index, pageSize);
            return Ok(BaseResponseModel<PaginatedList<ResponseEnrollmentModel>>.OkDataResponse(result, "Retrieved course status list for user sucessfully"));
        }
        [HttpPost]
        public async Task<IActionResult> Test(string userId, string courseId, string paymentId)
        {
            await _enrollmentService.CreateEnrollment(userId, courseId, paymentId); 
            return Ok("Retrieved course status list for user sucessfully");
        }
    }
}