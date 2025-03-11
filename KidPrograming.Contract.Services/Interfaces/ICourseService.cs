using KidPrograming.Contract.Repositories.PaggingItems;
using KidProgramming.ModelViews.ModelViews.CourseModels;

namespace KidPrograming.Contract.Services.Interfaces
{
    public interface ICourseService
    {
        Task<PaginatedList<ResponseCourseModel>> GetPage(bool? sortByTitle, bool? sortByPrice, string? searchById, string? searchByTitle, string? searchBySubject, string? teacherName, decimal? minPrice, decimal? maxPrice, int index, int pageSize);

        Task Create(CreateCourseModel model);

        Task Update(string id, UpdateCourseModel model);

        Task Delete(string id);
    }
}