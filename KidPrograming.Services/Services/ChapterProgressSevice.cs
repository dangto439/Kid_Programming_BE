using AutoMapper;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core;
using KidPrograming.Core.Constants;
using KidPrograming.Entity;
using KidPrograming.Services.Infrastructure;
using KidProgramming.ModelViews.ModelViews.ChapterProgressModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace KidPrograming.Services.Services
{
    public class ChapterProgressSevice : IChapterProgressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Authentication _authentication;
        private IHttpContextAccessor _httpContextAccessor;

        public ChapterProgressSevice(IUnitOfWork unitOfWork, IMapper mapper, Authentication authentication, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _authentication = authentication;
            _httpContextAccessor = httpContextAccessor;
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

        //public async Task UpdateAsync(string id, UpdateChapterProgressModel model)
        //{
        //    var entity = await _unitOfWork.GetRepository<ChapterProgress>().Entities.FirstOrDefaultAsync(x => x.Id == id);
        //    if (entity == null) throw new KeyNotFoundException("Chapter progress not found");

        //    _mapper.Map(model, entity);
        //    entity.LastUpdatedTime = DateTime.UtcNow;

        //    await _unitOfWork.GetRepository<ChapterProgress>().UpdateAsync(entity);
        //    await _unitOfWork.SaveAsync();
        //}

        public async Task DeleteAsync(string id)
        {
            var entity = await _unitOfWork.GetRepository<ChapterProgress>().Entities.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) throw new KeyNotFoundException("Chapter progress not found");

            entity.DeletedTime = DateTime.UtcNow;
            await _unitOfWork.GetRepository<ChapterProgress>().UpdateAsync(entity);
            await _unitOfWork.SaveAsync();
        }

        // ==================== //
        public async Task UpdateChapterProgressAsync(string chapterProgressId, int score)
        {
            ChapterProgress chapterProgress = await _unitOfWork.GetRepository<ChapterProgress>().GetByIdAsync(chapterProgressId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "ChapterProgress not found");

            if (score == 10)
            {
                chapterProgress.Progress = 100;
                chapterProgress.Status = Enums.ProgressStatus.Completed.ToString();
            }
            else
            {
                chapterProgress.Status = Enums.ProgressStatus.InProgress.ToString();
            }

            chapterProgress.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateLastAccessedAsync(string chapterId)
        {
            string userId = _authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

            Enrollment enrollment = await _unitOfWork.GetRepository<Enrollment>().Entities.FirstOrDefaultAsync(e => e.UserId == userId && e.Course.Chapters.Any(c => c.Id == chapterId) && !e.DeletedTime.HasValue) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Enrollment not found");

            ChapterProgress chapterProgress = await _unitOfWork.GetRepository<ChapterProgress>().Entities.FirstOrDefaultAsync(cp => cp.EnrollmentId == enrollment.Id && cp.ChapterId == chapterId && !cp.DeletedTime.HasValue) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "ChapterProgress not found");

            chapterProgress.Status = Enums.ProgressStatus.InProgress.ToString();
            chapterProgress.LastAccessed = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<ChapterProgress>().UpdateAsync(chapterProgress);
            await _unitOfWork.SaveAsync();
        }

        public async Task CheckFinalLab(string chapterProgressId, int score)
        {
            ChapterProgress chapterProgress = await _unitOfWork.GetRepository<ChapterProgress>()
                .Entities
                .Include(cp => cp.Chapter) // Ensure Chapter is loaded
                .Include(cp => cp.Enrollment) // Ensure Enrollment is loaded
                .FirstOrDefaultAsync(cp => cp.Id == chapterProgressId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "ChapterProgress not found");


            // Kiểm tra xem có phải chương cuối không
            bool isFinalChapter = await _unitOfWork.GetRepository<Chapter>()
                .Entities
                .Where(c => c.CourseId == chapterProgress.Chapter.CourseId)
                .OrderByDescending(c => c.Order)
                .Select(c => c.Id)
                .AnyAsync(id => id == chapterProgress.ChapterId);

            if (isFinalChapter && score == 10)
            {
                var enrollment = await _unitOfWork.GetRepository<Enrollment>().GetByIdAsync(chapterProgress.EnrollmentId);
                if (enrollment != null)
                {
                    enrollment.Status = Enums.StatusEnrollment.Completed.ToString();
                    enrollment.LastUpdatedTime = CoreHelper.SystemTimeNow;
                    await _unitOfWork.SaveAsync();
                }
            }
        }

        public async Task<List<InforProgressModel>> GetInforProgressAsync(string userId)
        {
            var enrollment = _unitOfWork.GetRepository<Enrollment>().Entities
                .Where(x => x.UserId == userId && x.DeletedTime == null)
                .Include(x => x.ChapterProgresses)
                .ThenInclude(x => x.Chapter)
                .Include(x => x.ChapterProgresses)
                .ThenInclude(cp => cp.Submission)
                .ThenInclude(x => x.Lab);
            var result = new List<InforProgressModel>();
            foreach (var en in enrollment)
            {
                string currentChapterTitle = en.ChapterProgresses
                     .Where(c => c.Status != Enums.ProgressStatus.InProgress.ToString())
                     .OrderBy(x => x.Chapter.Order)
                     .FirstOrDefault()?.Chapter.Title ?? "Completed";

                List<LabScore> labScores = new();
                foreach (var cp in en.ChapterProgresses)
                {
                    if(cp.Submission != null) { 
                    labScores.Add(new LabScore
                    {
                        LabTitle = cp.Submission.Lab.Title,
                        Score = cp.Submission.Score
                    });
                    }
                }
                result.Add(new InforProgressModel
                {
                    CurrentChapterTitle = currentChapterTitle,
                    LabScores = labScores
                });
            }
            return result;
        }

        public async Task<decimal> CalculateTotalProgress(string courseId, string userId)
        {
            bool courseExists = await _unitOfWork.GetRepository<Course>().Entities.AnyAsync(c => c.Id == courseId);
            if (!courseExists)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Course not found");
            }

            bool isEnrolled = await _unitOfWork.GetRepository<Enrollment>().Entities
                .AnyAsync(e => e.CourseId == courseId && e.UserId == userId && !e.DeletedTime.HasValue);

            if (!isEnrolled)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "This user hasn't enroll into the course");
            }

            var totalChapters = await _unitOfWork.GetRepository<Chapter>().Entities
                .Where(c => c.CourseId == courseId)
                .CountAsync();

            if (totalChapters == 0) return 0;

            int completedChapters = await _unitOfWork.GetRepository<ChapterProgress>().Entities
                .Where(cp => cp.Enrollment != null
                         && cp.Enrollment.UserId == userId
                         && cp.Chapter.CourseId == courseId
                         && cp.Status == Enums.ProgressStatus.Completed.ToString())
                .CountAsync();

            decimal progress = ((decimal)completedChapters / totalChapters) * 100;

            return Math.Round(progress, 2);
        }
    }
}