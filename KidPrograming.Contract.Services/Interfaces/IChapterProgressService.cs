using KidPrograming.Contract.Repositories.PaggingItems;
using KidProgramming.ModelViews.ModelViews.ChapterProgressModels;
using KidProgramming.ModelViews.ModelViews.LabModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        Task UpdateAsync(string id, UpdateChapterProgressModel model);
        Task DeleteAsync(string id);
    }
}
