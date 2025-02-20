using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core;
using KidPrograming.Core.Base;
using KidProgramming.ModelViews.ModelViews.CourseModels;
using Microsoft.AspNetCore.Mvc;

namespace KidPrograming.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        
        [HttpPost]
        public async Task<IActionResult> Create(CreateCourseModel model)
        {
            await _courseService.Create(model);
            return Ok(BaseResponse.OkMessageResponse("Created successfully"));

        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            await _courseService.Delete(id);
            return Ok(BaseResponse.OkMessageResponse("Deleted successfully"));
        }
    }
}
