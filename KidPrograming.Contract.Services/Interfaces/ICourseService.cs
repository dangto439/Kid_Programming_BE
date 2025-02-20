using KidProgramming.ModelViews.ModelViews.CourseModels;

namespace KidPrograming.Contract.Services.Interfaces
{
    public interface ICourseService
    {
        Task Create(CreateCourseModel model);
        Task Update(string id, UpdateCourseModel model);
        Task Delete(string id);
    }
}