using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core.Base;
using KidProgramming.ModelViews.ModelViews.CourseModels;
using Microsoft.AspNetCore.Mvc;
using static KidPrograming.Core.Constants.Enums;

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

        [HttpGet]
        public async Task<IActionResult> Get(bool? sortByTitle, bool? sortByPrice, CourseStatus? filterByStatus, string? searchByTitle, string? searchByPrice, string? teacherName, decimal? minPrice, decimal? maxPrice, int index = 1, int pageSize = 5)
        {
            PaginatedList<ResponseCourseModel> result = await _courseService.GetPage(sortByTitle, sortByPrice, filterByStatus, searchByTitle, searchByPrice, teacherName, minPrice, maxPrice, index, pageSize);
            return Ok(BaseResponseModel<PaginatedList<ResponseCourseModel>>.OkDataResponse(result, "Retrieve course list successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCourseModel model)
        {
            await _courseService.Create(model);
            return Ok(BaseResponse.OkMessageResponse("Created successfully"));
        }

        [HttpPut]
        public async Task<IActionResult> Update(string id, UpdateCourseModel model)
        {
            await _courseService.Update(id, model);
            return Ok(BaseResponse.OkMessageResponse("Updated successfully"));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            await _courseService.Delete(id);
            return Ok(BaseResponse.OkMessageResponse("Deleted successfully"));
        }
    }
}