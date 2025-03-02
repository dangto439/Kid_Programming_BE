using KidPrograming.Contract.Repositories.PaggingItems;
using KidProgramming.ModelViews.ModelViews.LabModels;
using KidProgramming.ModelViews.ModelViews.LessonModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidPrograming.Contract.Services.Interfaces
{
    public interface ILabService
    {
        Task<PaginatedList<ResponseLabModel>> GetPageAsync(
        string? searchById = null,
        string? lessonId = null,       
        string? searchByTitle = null,  
        string? searchByQuestion = null, 
        bool? sortByTitle = null,      
        bool? sortByResult = null,     
        int index = 1,                
        int pageSize = 10             
    );

        Task CreateAsync(CreateLabModel model);
        Task UpdateAsync(string id, UpdateLabModel model);
        Task DeleteAsync(string id);
    }
}
