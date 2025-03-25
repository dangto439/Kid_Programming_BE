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

        /// <summary>
        /// Get submissions list (search optional)
        /// </summary>
        /// <param name="searchById"></param>
        /// <param name="userId"></param>
        /// <param name="labId"></param>
        /// <param name="chapterProgressId"></param>
        /// <param name="minScore"></param>
        /// <param name="maxScore"></param>
        /// <param name="sortByScore"></param>
        /// <param name="sortByTimeSpent"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
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

        [HttpGet]
        [Route("creative-answer")]
        public async Task<IActionResult> GetCreativeSubmission(string userId, string courseId)
        {
            var result = await _submissionService.GetCreativeSubmissionAsync(userId, courseId);
            return Ok(BaseResponseModel<ResponseCreativeModel>.OkDataResponse(result, "Retrieve Creative Submission successfully"));
        }

        /// <summary>
        /// Create Submission (no need to use it instead of using api/submission/submit-lab)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSubmissionModel model)
        {
            await _submissionService.CreateAsync(model);
            await _cacheService.RemoveCacheResponseAsync("/api/submissions");
            return Ok(BaseResponse.OkMessageResponse("Created successfully"));
        }

        /// <summary>
        /// Update Submission by submissionId (don't need to use)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateSubmissionModel model)
        {
            await _submissionService.UpdateAsync(id, model);
            await _cacheService.RemoveCacheResponseAsync("/api/submissions");
            return Ok(BaseResponse.OkMessageResponse("Updated successfully"));
        }

        /// <summary>
        /// Update score for submission (can be used by Teacher to grade Creative-Lab)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("score")]
        public async Task<IActionResult> UpdateScore(string id, int score)
        {
            await _submissionService.UpdateScoreAsync(id, score);
            await _cacheService.RemoveCacheResponseAsync("/api/submissions");
            return Ok(BaseResponse.OkMessageResponse("Updated score sucessfully"));
        }

        /// <summary>
        /// To be used when student press submit the lab
        /// </summary>
        /// <param name="labId"></param>
        /// <param name="timeSpent"></param>
        /// <param name="answerCheck"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("submit-lab")]
        public async Task<IActionResult> SubmitLab(string labId, int timeSpent, string? answerCheck)
        {
            await _submissionService.SubmitLab(labId, timeSpent, answerCheck);
            await _cacheService.RemoveCacheResponseAsync("/api/submissions");
            return Ok(BaseResponse.OkMessageResponse($"Created submission for lab: {labId} sucessfully "));
        }


        /// <summary>
        /// Delete submission by submissionId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            await _submissionService.DeleteAsync(id);
            return Ok(BaseResponse.OkMessageResponse("Deleted successfully"));
        }
    }
}