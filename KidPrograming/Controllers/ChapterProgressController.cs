using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core.Base;
using KidProgramming.ModelViews.ModelViews.ChapterProgressModels;
using KidProgramming.ModelViews.ModelViews.LabModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KidPrograming.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChapterProgressController : ControllerBase
    {
        private readonly IChapterProgressService _chapterProgressService;

        public ChapterProgressController(IChapterProgressService chapterProgressService)
        {
            _chapterProgressService = chapterProgressService;
        }

        [HttpGet]
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
            return Ok(BaseResponse.OkMessageResponse("Created successfully"));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] UpdateChapterProgressModel model)
        {
            await _chapterProgressService.UpdateAsync(id, model);
            return Ok(BaseResponse.OkMessageResponse("Updated successfully"));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await _chapterProgressService.DeleteAsync(id);
            return Ok(BaseResponse.OkMessageResponse("Deleted successfully"));
        }
    }
}
