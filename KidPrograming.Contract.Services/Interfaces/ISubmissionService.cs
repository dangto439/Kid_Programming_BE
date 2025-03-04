using KidPrograming.Contract.Repositories.PaggingItems;
using KidProgramming.ModelViews.ModelViews.SubmissionModels;

namespace KidPrograming.Contract.Services.Interfaces
{
    public interface ISubmissionService
    {
        Task<PaginatedList<ResponseSubmissionModel>> GetPageAsync(
            string? searchById = null,
            string? userId = null,
            string? labId = null,
            string? chapterProgressId = null,
            int? minScore = null,
            int? maxScore = null,
            bool? sortByScore = null,
            bool? sortBySubmittedTime = null,
            int pageIndex = 1,
            int pageSize = 10
        );

        Task CreateAsync(CreateSubmissionModel model);

        Task UpdateAsync(string id, UpdateSubmissionModel model);

        Task DeleteAsync(string id);

        Task UpdateScoreAsync(string submissionId, int score);
    }
}