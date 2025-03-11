using KidPrograming.Attributes;
using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core.Base;
using KidProgramming.ModelViews.ModelViews.ChapterProgressModels;
using KidProgramming.ModelViews.ModelViews.LabModels;
using Microsoft.AspNetCore.Mvc;

namespace KidPrograming.Controllers
{
    [Route("api/chapter-progress")]
    [ApiController]
    public class ChapterProgressController : ControllerBase
    {
        private readonly IChapterProgressService _chapterProgressService;
        private readonly ICacheService _cacheService;

        public ChapterProgressController(IChapterProgressService chapterProgressService, ICacheService cacheService)
        {
            _chapterProgressService = chapterProgressService;
            _cacheService = cacheService;
        }

        [HttpGet]
        [CacheAtribute(1000)]
        public async Task<IActionResult> GetPageAsync(
           [FromQuery] string? searchById,
           [FromQuery] string? chapterId,
           [FromQuery] string? enrollmentId,
           [FromQuery] string? searchKeyword,
           [FromQuery] bool? sortByProgress,
           [FromQuery] bool? sortByLastAccessed,
           [FromQuery] int pageIndex = 1,
           [FromQuery] int pageSize = 10)
        {
            var result = await _chapterProgressService.GetPageAsync(
                searchById, chapterId, enrollmentId, searchKeyword,
                sortByProgress, sortByLastAccessed, pageIndex, pageSize);

            return Ok(BaseResponseModel<PaginatedList<ResponseLabModel>>.OkDataResponse(result, "Retrieve ChapterProgress list successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateChapterProgressModel model)
        {
            await _chapterProgressService.CreateAsync(model);
            await _cacheService.RemoveCacheResponseAsync("/api/chapter-progress");
            return Ok(BaseResponse.OkMessageResponse("Created successfully"));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateChapterProgressModel model)
        {
            await _chapterProgressService.UpdateAsync(id, model);
            await _cacheService.RemoveCacheResponseAsync("/api/chapter-progress");
            return Ok(BaseResponse.OkMessageResponse("Updated successfully"));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await _chapterProgressService.DeleteAsync(id);
            await _cacheService.RemoveCacheResponseAsync("/api/chapter-progress");
            return Ok(BaseResponse.OkMessageResponse("Deleted successfully"));
        }
    }
}