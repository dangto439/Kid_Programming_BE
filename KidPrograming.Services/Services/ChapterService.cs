using AutoMapper;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.ChapterModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace KidPrograming.Services
{
    public class ChapterService : IChapterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChapterService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task Create(CreateChapterModel model)
        {
            model.Validate();

            bool courseExist = await _unitOfWork.GetRepository<Course>().Entities.AnyAsync(x => x.Id == model.CourseId);

            if (!courseExist)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Course not found");
            }

            bool exists = await _unitOfWork.GetRepository<Chapter>().Entities.AnyAsync(x => x.CourseId == model.CourseId && x.Order == model.Order && !x.DeletedTime.HasValue);

            if (exists)
            {
                throw new ErrorException(StatusCodes.Status409Conflict, ResponseCodeConstants.EXISTED, $"Chapter {(model.Title)} has order duplicated");
            }

            Chapter newChapter = _mapper.Map<Chapter>(model);

            newChapter.CreatedTime = CoreHelper.SystemTimeNow;
            newChapter.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Chapter>().InsertAsync(newChapter);
            await _unitOfWork.SaveAsync();
        }

        public async Task Delete(string id)
        {
            Chapter chapter = await _unitOfWork.GetRepository<Chapter>().Entities.FirstOrDefaultAsync(x => x.Id == id) ??
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Chapter not found");

            chapter.DeletedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Chapter>().UpdateAsync(chapter);
            await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<ResponseChapterModel>> GetPage(string courseId, string? searchById, string? searchByTitle, bool? sortByOrder, int index, int pageSize)
        {
            IQueryable<ResponseChapterModel> query = _unitOfWork.GetRepository<Chapter>().Entities
                .Where(chapter => chapter.CourseId == courseId && !chapter.DeletedTime.HasValue)
                .Select(chapter => new ResponseChapterModel
                {
                    Id = chapter.Id,
                    Title = chapter.Title,
                    Description = chapter.Description,
                    Order = chapter.Order,
                    CreatedTime = chapter.CreatedTime
                });

            if (!string.IsNullOrWhiteSpace(searchById))
            {
                query = query.Where(x => x.Id.Equals(searchById));
            }

            if (!string.IsNullOrWhiteSpace(searchByTitle))
            {
                query = query.Where(x => EF.Functions.Like(x.Title, $"%{searchByTitle.Trim()}%"));
            }

            if (sortByOrder.HasValue)
            {
                query = sortByOrder.Value ? query.OrderBy(x => x.Order) : query.OrderByDescending(x => x.Order);
            }


            PaginatedList<ResponseChapterModel> paginatedCourses = await _unitOfWork.GetRepository<ResponseChapterModel>().GetPagging(query, index, pageSize);
            return paginatedCourses;
        }

        public async Task Update(string id, UpdateChapterModel model)
        {
            model.Validate();

            Chapter chapter = await _unitOfWork.GetRepository<Chapter>().Entities.FirstOrDefaultAsync(x => x.Id == id) ??
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Chapter not found");

            bool isDuplicateOrder = await _unitOfWork.GetRepository<Chapter>().Entities.AnyAsync(x => x.CourseId == chapter.CourseId && x.Order == model.Order && x.Id != chapter.Id && !x.DeletedTime.HasValue);

            if (isDuplicateOrder)
            {
                throw new ErrorException(StatusCodes.Status409Conflict, ResponseCodeConstants.EXISTED, $"Order {model.Order} has existed");
            }

            _mapper.Map(model, chapter);

            chapter.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Chapter>().UpdateAsync(chapter);
            await _unitOfWork.SaveAsync();
        }
    }
}
