using KidPrograming.Attributes;
using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core.Base;
using KidProgramming.ModelViews.ModelViews.LabModels;
using Microsoft.AspNetCore.Mvc;

namespace KidPrograming.Controllers
{
    [Route("api/labs")]
    [ApiController]
    public class LabController : ControllerBase
    {
        private readonly ILabService _labService;
        private readonly ICacheService _cacheService;

        public LabController(ILabService labService, ICacheService cacheService)
        {
            _labService = labService;
            _cacheService = cacheService;
        }

        [HttpGet]
        [CacheAtribute(1000)]
        public async Task<IActionResult> Get(
            [FromQuery] string? searchById,
            [FromQuery] string? chapterId,
            [FromQuery] string? searchByTitle,
            [FromQuery] string? searchByQuestion,
            [FromQuery] bool? sortByTitle,
            [FromQuery] bool? sortByResult,
            [FromQuery] int index = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _labService.GetPageAsync(searchById, chapterId, searchByTitle, searchByQuestion, sortByTitle, sortByResult, index, pageSize);
            return Ok(BaseResponseModel<PaginatedList<ResponseLabModel>>.OkDataResponse(result, "Retrieve lab list successfully"));
        }

        [HttpGet]
        [Route("/api/labs/{labId}/retrieve-answer")]
        public async Task<IActionResult> GetAnswer(string labId)
        {
            string answer =  await _labService.GetAnswerByLabIdAsync(labId);
            await _cacheService.RemoveCacheResponseAsync("/api/labs");
            return Ok(BaseResponseModel<string>.OkDataResponse(answer, "Answer is retrieved sucessfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLabModel model)
        {
            await _labService.CreateAsync(model);
            await _cacheService.RemoveCacheResponseAsync("/api/labs");
            return Ok(BaseResponse.OkMessageResponse("Created successfully"));
        }

        [HttpPut]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateLabModel model)
        {
            await _labService.UpdateAsync(id, model);
            await _cacheService.RemoveCacheResponseAsync("/api/labs");
            return Ok(BaseResponse.OkMessageResponse("Updated successfully"));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            await _labService.DeleteAsync(id);
            await _cacheService.RemoveCacheResponseAsync("/api/labs");
            return Ok(BaseResponse.OkMessageResponse("Deleted successfully"));
        }
    }
}