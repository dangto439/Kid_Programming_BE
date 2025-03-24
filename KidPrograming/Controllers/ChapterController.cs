using KidPrograming.Attributes;
using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core.Base;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.ChapterModels;
using KidProgramming.ModelViews.ModelViews.NotificationModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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

        /// <summary>
        /// Get chapter list of a course
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="searchById"></param>
        /// <param name="searchByTitle"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [CacheAtribute(1000)]
        public async Task<IActionResult> Get([Required] string courseId, string? searchById, string? searchByTitle, int index = 1, int pageSize = 10)
        {
            PaginatedList<ResponseChapterModel> result = await _chapterService.GetPage(courseId, searchById, searchByTitle, index, pageSize);
            return Ok(BaseResponseModel<PaginatedList<ResponseNotificationModel>>.OkDataResponse(result, "Retrieve chapter list successfully"));
        }

        /// <summary>
        /// Create chapters for the course
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateChapterModel model)
        {
            await _chapterService.Create(model);
            await _cacheService.RemoveCacheResponseAsync("/api/chapters");
            return Ok(BaseResponse.OkMessageResponse("Created sucessfully"));
        }

        /// <summary>
        /// Update chapter by chapterId
        /// </summary>
        /// <param name="chapterid"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Update(string chapterid, UpdateChapterModel model)
        {
            await _chapterService.Update(chapterid, model);
            await _cacheService.RemoveCacheResponseAsync("/api/chapters");
            return Ok(BaseResponse.OkMessageResponse("Updated sucessfully"));
        }

        /// <summary>
        /// Delete chapter by chapterId
        /// </summary>
        /// <param name="chapterid"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(string chapterid)
        {
            await _chapterService.Delete(chapterid);
            await _cacheService.RemoveCacheResponseAsync("/api/chapters");
            return Ok(BaseResponse.OkMessageResponse("Deleted sucessfully"));
        }
    }
}