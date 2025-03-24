using KidPrograming.Attributes;
using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core.Base;
using KidProgramming.ModelViews.ModelViews.CourseModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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

        /// <summary>
        /// Get Course list (search optional)
        /// </summary>
        /// <param name="sortByTitle"></param>
        /// <param name="sortByPrice"></param>
        /// <param name="searchById"></param>
        /// <param name="searchByTitle"></param>
        /// <param name="searchBySubject"></param>
        /// <param name="teacherId"></param>
        /// <param name="teacherName"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [CacheAtribute(1000)]
        public async Task<IActionResult> Get(bool? sortByTitle, bool? sortByPrice, string? searchById, string? searchByTitle, string? searchBySubject, string? teacherId, string? teacherName, decimal? minPrice, decimal? maxPrice, int index = 1, int pageSize = 10)
        {
            PaginatedList<ResponseCourseModel> result = await _courseService.GetPage(sortByTitle, sortByPrice, searchById, searchByTitle, searchBySubject, teacherId, teacherName, minPrice, maxPrice, index, pageSize);
            return Ok(BaseResponseModel<PaginatedList<ResponseCourseModel>>.OkDataResponse(result, "Retrieve course list successfully"));
        }

        /// <summary>
        /// Create Course
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateCourseModel model)
        {
            await _courseService.Create(model);
            await _cacheService.RemoveCacheResponseAsync("/api/courses");
            return Ok(BaseResponse.OkMessageResponse("Created successfully"));
        }

        /// <summary>
        /// Update Course by courseId
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Update([Required] string courseId, UpdateCourseModel model)
        {
            await _courseService.Update(courseId, model);
            await _cacheService.RemoveCacheResponseAsync("/api/courses");
            return Ok(BaseResponse.OkMessageResponse("Updated successfully"));
        }

        /// <summary>
        /// Delete Course By courseId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete([Required] string courseId)
        {
            await _courseService.Delete(courseId);
            await _cacheService.RemoveCacheResponseAsync("/api/courses");
            return Ok(BaseResponse.OkMessageResponse("Deleted successfully"));
        }
    }
}