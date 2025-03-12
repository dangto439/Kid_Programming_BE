using AutoMapper;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.LessonModels;
using Microsoft.AspNetCore.Http;
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
            bool? sortByTitle,
            string? searchByTitle,
            string? searchByContent,
            string? searchById,
            string chapterId,
            int index,
            int pageSize)
        {
            var query = _unitOfWork.GetRepository<Lesson>().Entities.Where(l => !l.DeletedTime.HasValue);

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

            var lessons = await query
                .OrderBy(l => l.Order)
                .ToListAsync();

            var mappedLessons = _mapper.Map<List<ResponseLessonModel>>(lessons);

            int totalCount = mappedLessons.Count;

            var pagedLessons = mappedLessons
                .Skip((index - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<ResponseLessonModel>(pagedLessons, totalCount, index, pageSize);
        }

        public async Task CreateAsync(CreateLessonModel model)
        {
            model.Validate();

            bool chapterExist = await _unitOfWork.GetRepository<Chapter>().Entities.AnyAsync(x => x.Id == model.ChapterId && !x.DeletedTime.HasValue);

            if (!chapterExist)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Chapter not found");
            }

            bool exists = await _unitOfWork.GetRepository<Lesson>().Entities.AnyAsync(x => x.ChapterId == model.ChapterId && x.Order == model.Order && !x.DeletedTime.HasValue);

            if (exists)
            {
                throw new ErrorException(StatusCodes.Status409Conflict, ResponseCodeConstants.EXISTED, $"Lesson {(model.Title)} has order duplicated");
            }

            Lesson lesson = _mapper.Map<Lesson>(model);
            lesson.CreatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Lesson>().InsertAsync(lesson);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(string id, UpdateLessonModel model)
        {
            model.Validate();

            Lesson lesson = await _unitOfWork.GetRepository<Lesson>().Entities.FirstOrDefaultAsync(x => x.Id == id && !x.DeletedTime.HasValue) 
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Lesson not found");

            bool exists = await _unitOfWork.GetRepository<Lesson>().Entities.AnyAsync(x => x.ChapterId == lesson.ChapterId && x.Order == model.Order && !x.DeletedTime.HasValue);

            if (exists)
            {
                throw new ErrorException(StatusCodes.Status409Conflict, ResponseCodeConstants.EXISTED, $"Lesson {(model.Title)} has order duplicated");
            }

            _mapper.Map(model, lesson);
            lesson.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Lesson>().UpdateAsync(lesson);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(string id)
        {
            Lesson lesson = await _unitOfWork.GetRepository<Lesson>().Entities.FirstOrDefaultAsync(x => x.Id == id && !x.DeletedTime.HasValue) ?? 
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Lesson not found");
            
            lesson.DeletedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Lesson>().UpdateAsync(lesson);
            await _unitOfWork.SaveAsync();
        }
    }
}
