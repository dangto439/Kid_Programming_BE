using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core.Base;
using KidProgramming.ModelViews.ModelViews.LabModels;
using KidProgramming.ModelViews.ModelViews.SubmissionModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KidPrograming.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmissionController : ControllerBase
    {
        private readonly ISubmissionService _submissionService;

        public SubmissionController(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        [HttpGet]
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
            var result = await _submissionService.GetPageAsync(searchById, userId, labId, chapterProgressId,minScore, maxScore, sortByScore, sortByTimeSpent, pageIndex, pageSize);
            return Ok(BaseResponseModel<PaginatedList<ResponseSubmissionModel>>.OkDataResponse(result, "Retrieve Submission list successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSubmissionModel model)
        {
            await _submissionService.CreateAsync(model);
            return Ok(BaseResponse.OkMessageResponse("Created successfully"));
        }

        [HttpPut]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateSubmissionModel model)
        {
            await _submissionService.UpdateAsync(id, model);
            return Ok(BaseResponse.OkMessageResponse("Updated successfully"));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            await _submissionService.DeleteAsync(id);
            return Ok(BaseResponse.OkMessageResponse("Deleted successfully"));
        }
    }
}
