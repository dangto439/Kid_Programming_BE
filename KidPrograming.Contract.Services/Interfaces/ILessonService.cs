using KidPrograming.Contract.Repositories.PaggingItems;
using KidProgramming.ModelViews.ModelViews.LessonModels;

namespace KidPrograming.Contract.Services.Interfaces
{
    public interface ILessonService
    {
        Task<PaginatedList<ResponseLessonModel>> GetPageAsync(
            bool? sortByTitle,
            string? searchByTitle,
            string? searchByContent,
            string? searchById,
            string chapterId,
            int index,
            int pageSize
        );

        Task CreateAsync(CreateLessonModel model);

        Task UpdateAsync(string id, UpdateLessonModel model);

        Task DeleteAsync(string id);
    }
}