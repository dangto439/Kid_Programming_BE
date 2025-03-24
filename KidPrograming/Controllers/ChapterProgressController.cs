using KidPrograming.Attributes;
using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core.Base;
using KidProgramming.ModelViews.ModelViews.ChapterProgressModels;
using KidProgramming.ModelViews.ModelViews.LabModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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

        /// <summary>
        /// Get ChapterProgress list (search optional)
        /// </summary>
        /// <param name="searchById"></param>
        /// <param name="chapterId"></param>
        /// <param name="enrollmentId"></param>
        /// <param name="searchKeyword"></param>
        /// <param name="sortByProgress"></param>
        /// <param name="sortByLastAccessed"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get progress for a course
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("calculate-progress")]
        public async Task<IActionResult> GetProgressTotal([Required] string courseId, [Required] string userId)
        {
            var result = await _chapterProgressService.CalculateTotalProgress(courseId, userId);
            return Ok(BaseResponseModel<decimal>.OkDataResponse(result, $"Retrieve progress of course: {courseId} on user: {userId} sucessfully"));
        }

        /// <summary>
        /// Create ChapterProgress (don't use bcs it has integrated auto insert into /api/vnpay when enroll successfully the course)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateChapterProgressModel model)
        {
            await _chapterProgressService.CreateAsync(model);
            await _cacheService.RemoveCacheResponseAsync("/api/chapter-progress");
            return Ok(BaseResponse.OkMessageResponse("Created successfully"));
        }

        /// <summary>
        /// Update Last Accessed of ChapterProgress for OnlineUser by ChapterId
        /// </summary>
        /// <param name="chapterId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("update-last-accessed/{chapterId}")]
        public async Task<IActionResult> UpdateLastAccessed(string chapterId)
        {
            await _chapterProgressService.UpdateLastAccessedAsync(chapterId);
            await _cacheService.RemoveCacheResponseAsync("/api/chapter-progress");
            return Ok(BaseResponse.OkMessageResponse("Updated successfully"));
        }

        /// <summary>
        /// Update Score of ChapterProgress by chapterProgressId (don't use, bcs it has integrated into /api/submission/submit-lab)
        /// </summary>
        /// <param name="chapterProgressId"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("update-score/{chapterProgressId}")]
        public async Task<IActionResult> UpdateProgress(string chapterProgressId, int score)
        {
            await _chapterProgressService.UpdateChapterProgressAsync(chapterProgressId, score);
            await _cacheService.RemoveCacheResponseAsync("api/chapter-progress");
            return Ok(BaseResponse.OkMessageResponse("Updated successfully"));
        }

        /// <summary>
        /// Delete ChapterProgress by ChapterProgressId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await _chapterProgressService.DeleteAsync(id);
            await _cacheService.RemoveCacheResponseAsync("/api/chapter-progress");
            return Ok(BaseResponse.OkMessageResponse("Deleted successfully"));
        }

        [HttpGet]
        [Route("infor-progress")]
        public async Task<IActionResult> GetInforProgress(string userId)
        {
            var results = await _chapterProgressService.GetInforProgressAsync(userId);
            return Ok(BaseResponseModel<PaginatedList<List<InforProgressModel>>>.OkDataResponse(results, "Retrieve infor list successfully"));
        }
    }
}