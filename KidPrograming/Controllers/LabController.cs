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

        public LabController(ILabService labService)
        {
            _labService = labService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string? searchById,
            [FromQuery] string? lessonId,
            [FromQuery] string? searchByTitle,
            [FromQuery] string? searchByQuestion,
            [FromQuery] bool? sortByTitle,
            [FromQuery] bool? sortByResult,
            [FromQuery] int index = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _labService.GetPageAsync(searchById, lessonId, searchByTitle, searchByQuestion, sortByTitle, sortByResult, index, pageSize);
            return Ok(BaseResponseModel<PaginatedList<ResponseLabModel>>.OkDataResponse(result, "Retrieve lab list successfully"));
        }

        [HttpGet]
        [Route("/api/labs/{labId}/retrieve-answer")]
        public async Task<IActionResult> GetAnswer(string labId)
        {
            string answer =  await _labService.GetAnswerByLabIdAsync(labId);
            return Ok(BaseResponseModel<string>.OkDataResponse(answer, "Answer is retrieved sucessfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLabModel model)
        {
            await _labService.CreateAsync(model);
            return Ok(BaseResponse.OkMessageResponse("Created successfully"));
        }

        [HttpPut]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateLabModel model)
        {
            await _labService.UpdateAsync(id, model);
            return Ok(BaseResponse.OkMessageResponse("Updated successfully"));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            await _labService.DeleteAsync(id);
            return Ok(BaseResponse.OkMessageResponse("Deleted successfully"));
        }
    }
}