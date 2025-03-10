using KidPrograming.Attributes;
using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core.Base;
using KidProgramming.ModelViews.ModelViews.SubmissionModels;
using Microsoft.AspNetCore.Mvc;

namespace KidPrograming.Controllers
{
    [Route("api/submissions")]
    [ApiController]
    public class SubmissionController : ControllerBase
    {
        private readonly ISubmissionService _submissionService;
        private readonly ICacheService _cacheService;

        public SubmissionController(ISubmissionService submissionService, ICacheService cacheService)
        {
            _submissionService = submissionService;
            _cacheService = cacheService;
        }

        [HttpGet]
        [CacheAtribute(1000)]
        public async Task<IActionResult> GetPageAsync(
            [FromQuery] string? searchById = null,
            [FromQuery] string? userId = null,
            [FromQuery] string? labId = null,
            [FromQuery] string? chapterProgressId = null,
            [FromQuery] int? minScore = null,
            [FromQuery] int? maxScore = null,
            [FromQuery] bool? sortByScore = null,
            [FromQuery] bool? sortByTimeSpent = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _submissionService.GetPageAsync(searchById, userId, labId, chapterProgressId, minScore, maxScore, sortByScore, sortByTimeSpent, pageIndex, pageSize);
            return Ok(BaseResponseModel<PaginatedList<ResponseSubmissionModel>>.OkDataResponse(result, "Retrieve Submission list successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSubmissionModel model)
        {
            await _submissionService.CreateAsync(model);
            await _cacheService.RemoveCacheResponseAsync("api/submissions");
            return Ok(BaseResponse.OkMessageResponse("Created successfully"));
        }

        [HttpPut]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateSubmissionModel model)
        {
            await _submissionService.UpdateAsync(id, model);
            await _cacheService.RemoveCacheResponseAsync("api/submissions");
            return Ok(BaseResponse.OkMessageResponse("Updated successfully"));
        }

        [HttpPatch]
        [Route("/api/submissions/score")]
        public async Task<IActionResult> UpdateScore(string id, int score)
        {
            await _submissionService.UpdateScoreAsync(id, score);
            await _cacheService.RemoveCacheResponseAsync("api/submissions");
            return Ok(BaseResponse.OkMessageResponse("Updated score sucessfully"));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            await _submissionService.DeleteAsync(id);
            return Ok(BaseResponse.OkMessageResponse("Deleted successfully"));
        }
    }
}