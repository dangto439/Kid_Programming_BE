using KidPrograming.Contract.Repositories.PaggingItems;
using KidProgramming.ModelViews.ModelViews.ChapterModels;

namespace KidPrograming.Contract.Services.Interfaces
{
    public interface IChapterService
    {
        Task<PaginatedList<ResponseChapterModel>> GetPage(string courseId, string? searchByTitle, bool? sortByOrder, int index, int pageSize);

        Task Create(CreateChapterModel model);

        Task Update(string id, UpdateChapterModel model);

        Task Delete(string id);
    }
}