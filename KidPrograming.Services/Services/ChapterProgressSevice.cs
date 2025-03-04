using AutoMapper;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.ChapterProgressModels;
using KidProgramming.ModelViews.ModelViews.LabModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidPrograming.Services.Services
{
    public class ChapterProgressSevice : IChapterProgressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChapterProgressSevice(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedList<ResponseChapterProgressModel>> GetPageAsync(
            string? searchById = null,
            string? chapterId = null,
            string? enrollmentId = null,
            string? searchKeyword = null,
            bool? sortByProgress = null,
            bool? sortByLastAccessed = null,
            int pageIndex = 1,
            int pageSize = 10)
        {
            var query = _unitOfWork.GetRepository<ChapterProgress>().Entities.Where(x => x.DeletedTime == null);

            if (!string.IsNullOrEmpty(searchById))
                query = query.Where(x => x.Id == searchById);

            if (!string.IsNullOrEmpty(chapterId))
                query = query.Where(x => x.ChapterId == chapterId);

            if (!string.IsNullOrEmpty(enrollmentId))
                query = query.Where(x => x.EnrollmentId == enrollmentId);

            if (!string.IsNullOrEmpty(searchKeyword))
                query = query.Where(x => x.Chapter.Title.Contains(searchKeyword) ||
                                         x.Enrollment.Id.Contains(searchKeyword));

            if (sortByProgress == true)
                query = query.OrderBy(x => x.Progress);
            else if (sortByProgress == false)
                query = query.OrderByDescending(x => x.Progress);

            if (sortByLastAccessed == true)
                query = query.OrderBy(x => x.LastAccessed);
            else if (sortByLastAccessed == false)
                query = query.OrderByDescending(x => x.LastAccessed);

            var mappedQuery = query.Select(l => _mapper.Map<ResponseChapterProgressModel>(l));
            return await _unitOfWork.GetRepository<ResponseChapterProgressModel>().GetPagging(mappedQuery, pageIndex, pageSize);
        }

        public async Task CreateAsync(CreateChapterProgressModel model)
        {
            model.Validate();

            var enroll = _unitOfWork.GetRepository<Enrollment>().Entities.FirstOrDefault(x => x.Id == model.EnrollmentId && x.DeletedTime == null);

            if (enroll == null)
            {
                throw new KeyNotFoundException("Enroll not found or has been delete");
            }

            var chapter = _unitOfWork.GetRepository<Chapter>().Entities.FirstOrDefault(x => x.Id == model.ChapterId && x.DeletedTime == null);

            if (enroll == null)
            {
                throw new KeyNotFoundException("Chapter not found or has been delete");
            }

            var entity = _mapper.Map<ChapterProgress>(model);
            entity.CreatedTime = DateTime.UtcNow;
            await _unitOfWork.GetRepository<ChapterProgress>().InsertAsync(entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(string id, UpdateChapterProgressModel model)
        {
            var entity = await _unitOfWork.GetRepository<ChapterProgress>().Entities.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) throw new KeyNotFoundException("Chapter progress not found");

            _mapper.Map(model, entity);
            entity.LastUpdatedTime = DateTime.UtcNow;

            await _unitOfWork.GetRepository<ChapterProgress>().UpdateAsync(entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _unitOfWork.GetRepository<ChapterProgress>().Entities.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) throw new KeyNotFoundException("Chapter progress not found");

            entity.DeletedTime = DateTime.UtcNow;
            await _unitOfWork.GetRepository<ChapterProgress>().UpdateAsync(entity);
            await _unitOfWork.SaveAsync();
        }
    }
}
