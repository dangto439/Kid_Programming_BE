using AutoMapper;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.LessonModels;
using Microsoft.EntityFrameworkCore;

namespace KidPrograming.Services.Services
{
    public class LessonService : ILessonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LessonService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedList<ResponseLessonModel>> GetPageAsync(
            bool? sortByTitle = null,
            bool? sortByOrder = null,
            string? searchByTitle = null,
            string? searchByContent = null,
            string? searchById = null,
            string? chapterId = null,
            int index = 1,
            int pageSize = 10)
        {
            var query = _unitOfWork.GetRepository<Lesson>().Entities.Where(l => l.DeletedTime == null);

            if (!string.IsNullOrEmpty(searchByTitle))
            {
                query = query.Where(l => l.Title.Contains(searchByTitle));
            }

            if (!string.IsNullOrEmpty(searchByContent))
            {
                query = query.Where(l => l.Content.Contains(searchByContent));
            }

            if (!string.IsNullOrEmpty(searchById))
            {
                query = query.Where(l => l.Id == searchById);
            }

            if (!string.IsNullOrEmpty(chapterId))
            {
                query = query.Where(l => l.ChapterId == chapterId);
            }

            if (sortByTitle == true)
            {
                query = query.OrderBy(l => l.Title);
            }
            else if (sortByTitle == false)
            {
                query = query.OrderByDescending(l => l.Title);
            }

            if (sortByOrder == true)
            {
                query = query.OrderBy(l => l.Order);
            }
            else if (sortByOrder == false)
            {
                query = query.OrderByDescending(l => l.Order);
            }

            var mappedQuery = query.Select(l => _mapper.Map<ResponseLessonModel>(l));
            return await _unitOfWork.GetRepository<ResponseLessonModel>().GetPagging(mappedQuery, index, pageSize);
        }

        public async Task CreateAsync(CreateLessonModel model)
        {
            model.Validate();
            var lessonExit = await _unitOfWork.GetRepository<Lesson>().Entities.FirstOrDefaultAsync(x => x.Title == model.Title);
            if (lessonExit != null)
            {
                throw new InvalidOperationException("Lesson has already Titilư");
            }
            var lesson = _mapper.Map<Lesson>(model);
            lesson.CreatedTime = DateTime.Now;
            await _unitOfWork.GetRepository<Lesson>().InsertAsync(lesson);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(string id, UpdateLessonModel model)
        {
            model.Validate();
            var lesson = await _unitOfWork.GetRepository<Lesson>().Entities.FirstOrDefaultAsync(x => x.Id == id);
            if (lesson == null) throw new KeyNotFoundException("Lesson not found");

            _mapper.Map(model, lesson);
            lesson.LastUpdatedTime = DateTime.UtcNow;
            await _unitOfWork.GetRepository<Lesson>().UpdateAsync(lesson);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var lesson = await _unitOfWork.GetRepository<Lesson>().Entities.FirstOrDefaultAsync(x => x.Id == id);
            if (lesson == null) throw new KeyNotFoundException("Lesson not found");
            if (lesson.DeletedTime != null)
            {
                throw new InvalidOperationException("Lesson has already been deleted");
            }
            lesson.DeletedTime = DateTime.Now;
            await _unitOfWork.GetRepository<Lesson>().UpdateAsync(lesson);
            await _unitOfWork.SaveAsync();
        }
    }
}
