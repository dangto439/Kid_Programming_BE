using KidPrograming.Contract.Repositories.PaggingItems;
using KidProgramming.ModelViews.ModelViews.LessonModels;

namespace KidPrograming.Contract.Services.Interfaces
{
    public interface ILessonService
    {
        Task<PaginatedList<ResponseLessonModel>> GetPageAsync(
            bool? sortByTitle = null,
            bool? sortByOrder = null,
            string? searchByTitle = null,
            string? searchByContent = null,
            string? searchById = null,
            string? chapterId = null,
            int index = 1,
            int pageSize = 10
        );

        Task CreateAsync(CreateLessonModel model);

        Task UpdateAsync(string id, UpdateLessonModel model);

        Task DeleteAsync(string id);
    }
}