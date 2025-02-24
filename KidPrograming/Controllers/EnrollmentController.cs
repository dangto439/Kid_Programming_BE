using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core.Base;
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
            return Ok(BaseResponseModel<string>.OkDataResponse(result, "Đăng nhập thành công"));
        }
    }
}
