using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.CourseModels;
using static KidPrograming.Core.Constants.Enums;

namespace KidPrograming.Contract.Services.Interfaces
{
    public interface ICourseService
    {
        Task<PaginatedList<ResponseCourseModel>> GetPage(bool? sortByTitle, bool? sortByPrice, CourseStatus? filterByStatus, string? searchById, string? searchByTitle, string? searchBySubject, string? teacherName, decimal? minPrice, decimal? maxPrice, int index, int pageSize);
        Task Create(CreateCourseModel model);

        Task Update(string id, UpdateCourseModel model);

        Task Delete(string id);
    }
}