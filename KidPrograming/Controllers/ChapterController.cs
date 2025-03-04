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
        private readonly IChapterService _chapterService;

        public ChapterController(IChapterService chapterService)
        {
            _chapterService = chapterService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string courseId, string? searchByTitle, bool? sortByOrder, int index = 1, int pageSize = 10)
        {
            PaginatedList<ResponseChapterModel> result = await _chapterService.GetPage(courseId, searchByTitle, sortByOrder, index, pageSize);
            return Ok(BaseResponseModel<PaginatedList<ResponseNotificationModel>>.OkDataResponse(result, "Retrieve chapter list successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateChapterModel model)
        {
            await _chapterService.Create(model);
            return Ok(BaseResponse.OkMessageResponse("Created sucessfully"));
        }

        [HttpPut]
        public async Task<IActionResult> Update(string id, UpdateChapterModel model)
        {
            await _chapterService.Update(id, model);
            return Ok(BaseResponse.OkMessageResponse("Updated sucessfully"));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            await _chapterService.Delete(id);
            return Ok(BaseResponse.OkMessageResponse("Deleted sucessfully"));
        }
    }
}