using KidPrograming.Contract.Repositories.PaggingItems;
using KidProgramming.ModelViews.ModelViews.EnrollmentModels;

namespace KidPrograming.Contract.Services.Interfaces
{
    public interface IEnrollmentService
    {
        public Task CreateEnrollment(string userId, string paymentId,string courseId);
        public Task<PaginatedList<StudentModel>> GetStudentByCourseId(string courseId, string? searchByUserName, int index = 1, int pageSize = 10);
    }
}
