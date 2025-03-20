using KidPrograming.Contract.Repositories.PaggingItems;
using KidProgramming.ModelViews.ModelViews.ChapterProgressModels;

namespace KidPrograming.Contract.Services.Interfaces
{
    public interface IChapterProgressService
    {
        Task<PaginatedList<ResponseChapterProgressModel>> GetPageAsync(
            string? searchById = null,
            string? chapterId = null,
            string? enrollmentId = null,
            string? searchKeyword = null,
            bool? sortByProgress = null,
            bool? sortByLastAccessed = null,
            int pageIndex = 1,
            int pageSize = 10
        );

        Task CreateAsync(CreateChapterProgressModel model);

        //Task UpdateAsync(string id, UpdateChapterProgressModel model);
        Task DeleteAsync(string id);

        Task UpdateChapterProgressAsync(string chapterProgressId, int score);

        Task UpdateLastAccessedAsync(string chapterId);

        Task CheckFinalLab(string chapterProgressId, int score);

        Task<List<InforProgressModel>> GetInforProgressAsync(string userId);

        Task<decimal> CalculateTotalProgress(string courseId, string userId);
    }
}