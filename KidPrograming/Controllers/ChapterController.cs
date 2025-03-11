using KidPrograming.Attributes;
using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core.Base;
using KidProgramming.ModelViews.ModelViews.ChapterModels;
using KidProgramming.ModelViews.ModelViews.NotificationModels;
using Microsoft.AspNetCore.Mvc;

namespace KidPrograming.Controllers
{
    [ApiController]
    [Route("/api/chapters")]
    public class ChapterController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly IChapterService _chapterService;

        public ChapterController(IChapterService chapterService, ICacheService cacheService)
        {
            _chapterService = chapterService;
            _cacheService = cacheService;
        }

        [HttpGet]
        [CacheAtribute(1000)]
        public async Task<IActionResult> Get(string courseId, string? searchById, string? searchByTitle, bool? sortByOrder, int index = 1, int pageSize = 10)
        {
            PaginatedList<ResponseChapterModel> result = await _chapterService.GetPage(courseId, searchById, searchByTitle, sortByOrder, index, pageSize);
            return Ok(BaseResponseModel<PaginatedList<ResponseNotificationModel>>.OkDataResponse(result, "Retrieve chapter list successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateChapterModel model)
        {
            await _chapterService.Create(model);
            await _cacheService.RemoveCacheResponseAsync("api/chapters");
            return Ok(BaseResponse.OkMessageResponse("Created sucessfully"));
        }

        [HttpPut]
        public async Task<IActionResult> Update(string id, UpdateChapterModel model)
        {
            await _chapterService.Update(id, model);
            await _cacheService.RemoveCacheResponseAsync("api/chapters");
            return Ok(BaseResponse.OkMessageResponse("Updated sucessfully"));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            await _chapterService.Delete(id);
            await _cacheService.RemoveCacheResponseAsync("api/chapters");
            return Ok(BaseResponse.OkMessageResponse("Deleted sucessfully"));
        }
    }
}