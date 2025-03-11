using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core.Base;
using KidPrograming.Attributes;
using KidProgramming.ModelViews.ModelViews.CourseModels;
using Microsoft.AspNetCore.Mvc;
using static KidPrograming.Core.Constants.Enums;

namespace KidPrograming.Controllers
{
    [ApiController]
    [Route("/api/courses")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ICacheService _cacheService;

        public CourseController(ICourseService courseService, ICacheService cacheService)
        {
            _courseService = courseService;
            _cacheService = cacheService;
        }

        [HttpGet]
        [CacheAtribute(1000)]
        public async Task<IActionResult> Get(bool? sortByTitle, bool? sortByPrice, string? searchById, string? searchByTitle, string? searchByPrice, string? teacherName, decimal? minPrice, decimal? maxPrice, int index = 1, int pageSize = 10)
        {
            PaginatedList<ResponseCourseModel> result = await _courseService.GetPage(sortByTitle, sortByPrice, searchById, searchByTitle, searchByPrice, teacherName, minPrice, maxPrice, index, pageSize);
            return Ok(BaseResponseModel<PaginatedList<ResponseCourseModel>>.OkDataResponse(result, "Retrieve course list successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCourseModel model)
        {
            await _cacheService.RemoveCacheResponseAsync("api/courses");
            await _courseService.Create(model);
            return Ok(BaseResponse.OkMessageResponse("Created successfully"));
        }

        [HttpPut]
        public async Task<IActionResult> Update(string id, UpdateCourseModel model)
        {
            await _cacheService.RemoveCacheResponseAsync("api/courses");
            await _courseService.Update(id, model);
            return Ok(BaseResponse.OkMessageResponse("Updated successfully"));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            await _cacheService.RemoveCacheResponseAsync("api/courses");
            await _courseService.Delete(id);
            return Ok(BaseResponse.OkMessageResponse("Deleted successfully"));
        }
    }
}