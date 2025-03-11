using KidPrograming.Attributes;
using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core.Base;
using KidProgramming.ModelViews.ModelViews.LessonModels;
using Microsoft.AspNetCore.Mvc;

namespace KidPrograming.Controllers
{
    [ApiController]
    [Route("/api/lessons")]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;
        private readonly ICacheService _cacheService;

        public LessonController(ILessonService lessonService, ICacheService cacheService)
        {
            _lessonService = lessonService;
            _cacheService = cacheService;
        }

        [HttpGet]
        [CacheAtribute(1000)]
        public async Task<IActionResult> Get(
            bool? sortByTitle,
            bool? sortByOrder,
            string? searchByTitle,
            string? searchByContent,
            string? searchById,
            string chapterId,
            int index = 1,
            int pageSize = 10)
        {
            PaginatedList<ResponseLessonModel> result = await _lessonService.GetPageAsync(
                sortByTitle, sortByOrder, searchByTitle, searchByContent, searchById,chapterId, index, pageSize);

            return Ok(BaseResponseModel<PaginatedList<ResponseLessonModel>>.OkDataResponse(result, "Retrieve lesson list successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLessonModel model)
        {
            await _lessonService.CreateAsync(model);
            await _cacheService.RemoveCacheResponseAsync("api/lessons");
            return Ok(BaseResponse.OkMessageResponse("Created successfully"));
        }

        [HttpPut]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateLessonModel model)
        {
            await _lessonService.UpdateAsync(id, model);
            await _cacheService.RemoveCacheResponseAsync("api/lessons");
            return Ok(BaseResponse.OkMessageResponse("Updated successfully"));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            await _lessonService.DeleteAsync(id);
            await _cacheService.RemoveCacheResponseAsync("api/lessons");
            return Ok(BaseResponse.OkMessageResponse("Deleted successfully"));
        }
    }
}
