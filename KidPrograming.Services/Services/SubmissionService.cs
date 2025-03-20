using AutoMapper;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core;
using KidPrograming.Core.Constants;
using KidPrograming.Entity;
using KidPrograming.Services.Infrastructure;
using KidProgramming.ModelViews.ModelViews.SubmissionModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace KidPrograming.Services.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuthenticationService _authenticationService;
        private readonly IChapterProgressService _chapterProgressService;
        private readonly Authentication _authentication;
        private readonly IHttpContextAccessor _contextAccessor;

        public SubmissionService(IUnitOfWork unitOfWork, IMapper mapper, IAuthenticationService authenticationService, IChapterProgressService chapterProgressService, Authentication authentication, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _authenticationService = authenticationService;
            _chapterProgressService = chapterProgressService;
            _authentication = authentication;
            _contextAccessor = contextAccessor;
        }

        public async Task<PaginatedList<ResponseSubmissionModel>> GetPageAsync(
            string? searchById = null,
            string? userId = null,
            string? labId = null,
            string? chapterProgressId = null,
            int? minScore = null,
            int? maxScore = null,
            bool? sortByScore = null,
            bool? sortByTimeSpent = null,
            int pageIndex = 1,
            int pageSize = 10)
        {
            var query = _unitOfWork.GetRepository<Submission>().Entities.Where(x => x.DeletedTime == null);

            if (!string.IsNullOrEmpty(searchById))
                query = query.Where(x => x.Id == searchById);

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(x => x.UserId == userId);

            if (!string.IsNullOrEmpty(labId))
                query = query.Where(x => x.LabId == labId);

            if (!string.IsNullOrEmpty(chapterProgressId))
                query = query.Where(x => x.ChapterProgressId == chapterProgressId);

            if (minScore.HasValue)
                query = query.Where(x => x.Score >= minScore);

            if (maxScore.HasValue)
                query = query.Where(x => x.Score <= maxScore);

            if (sortByScore == true)
                query = query.OrderBy(x => x.Score);
            else if (sortByScore == false)
                query = query.OrderByDescending(x => x.Score);

            if (sortByTimeSpent == true)
                query = query.OrderBy(x => x.TimeSpent);
            else if (sortByTimeSpent == false)
                query = query.OrderByDescending(x => x.TimeSpent);

            var mappedQuery = query.Select(l => _mapper.Map<ResponseSubmissionModel>(l));
            return await _unitOfWork.GetRepository<ResponseSubmissionModel>().GetPagging(mappedQuery, pageIndex, pageSize);
        }

        public async Task CreateAsync(CreateSubmissionModel model)
        {
            model.Validate();

            Lab lab = await _unitOfWork.GetRepository<Lab>().Entities.FirstOrDefaultAsync(x => x.Id == model.LabId && !x.DeletedTime.HasValue) ??
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Lab not found or has been deleted");

            ChapterProgress chapterProgress = await _unitOfWork.GetRepository<ChapterProgress>().Entities.FirstOrDefaultAsync(x => x.Id == model.ChapterProgressId && !x.DeletedTime.HasValue) ??
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "ChapterProgress not found or has been deleted");

            var user = _authenticationService.GetUserInfo();

            var submission = _mapper.Map<Submission>(model);
            submission.UserId = user.Result.Id!;
            submission.SubmittedTime = CoreHelper.SystemTimeNow;
            submission.CreatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Submission>().InsertAsync(submission);
            await _unitOfWork.SaveAsync();

        }

        public async Task UpdateAsync(string id, UpdateSubmissionModel model)
        {
            var submission = await _unitOfWork.GetRepository<Submission>().Entities.FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null);
            if (submission == null)
                throw new KeyNotFoundException("Submission not found");

            _mapper.Map(model, submission);
            submission.LastUpdatedTime = DateTimeOffset.UtcNow;
            await _unitOfWork.GetRepository<Submission>().UpdateAsync(submission);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var submission = await _unitOfWork.GetRepository<Submission>().Entities.FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null);
            if (submission == null)
                throw new KeyNotFoundException("Submission not found");

            submission.DeletedTime = DateTimeOffset.UtcNow;
            await _unitOfWork.GetRepository<Submission>().UpdateAsync(submission);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateScoreAsync(string submissionId, int score)
        {
            Submission submission = await _unitOfWork.GetRepository<Submission>().GetByIdAsync(submissionId) ??
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Submission not found");

            if (score > 10 || score < 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Score must be ranges from 0 to 10");
            }

            submission.Score = score;
            submission.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Submission>().UpdateAsync(submission);
            await _unitOfWork.SaveAsync();

            await _chapterProgressService.CheckFinalLab(submission.ChapterProgressId, score);
        }

        public async Task SubmitLab(string labId, int timeSpent, string? answerCheck)
        {
            string userId = _authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

            if (string.IsNullOrEmpty(userId))
            {
                throw new ErrorException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.NOT_FOUND, "User is not found");
            }

            bool labExists = await _unitOfWork.GetRepository<Lab>()
                .Entities
                .AnyAsync(l => l.Id == labId && !l.DeletedTime.HasValue);

            if (!labExists)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Lab is not found");
            }

            var chapterProgress = await _unitOfWork.GetRepository<ChapterProgress>()
                .Entities
                .Include(cp => cp.Submission)
                .Include(cp => cp.Chapter)
                .ThenInclude(c => c.Lab)
                .FirstOrDefaultAsync(cp =>
                    cp.Enrollment.UserId == userId &&
                    cp.Chapter.Lab.Id == labId);

            if (chapterProgress == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "ChapterProgress not found");
            }

            // Lấy correctAnswer của Lab
            string? correctAnswer = await _unitOfWork.GetRepository<Lab>()
                .Entities
                .Where(l => l.Id == labId)
                .Select(l => l.CorrectAnswer)
                .FirstOrDefaultAsync();

            bool isCreativeLab = string.IsNullOrEmpty(correctAnswer);
            int score = isCreativeLab ? 0 : string.Equals(correctAnswer?.Trim(), answerCheck?.Trim(), StringComparison.OrdinalIgnoreCase) ? 10 : 0;

            // Kiểm tra xem đã có Submission chưa
            Submission? existingSubmission = chapterProgress.Submission;

            if (existingSubmission != null)
            {
                // Cập nhật Submission hiện có
                existingSubmission.Score = score;
                existingSubmission.TimeSpent = timeSpent;
                existingSubmission.SubmittedTime = CoreHelper.SystemTimeNow;
                existingSubmission.CreativeAnswer = isCreativeLab ? answerCheck : string.Empty;

                await _unitOfWork.GetRepository<Submission>().UpdateAsync(existingSubmission);
            }
            else
            {
                // Tạo mới Submission
                var submission = new Submission
                {
                    UserId = userId,
                    LabId = labId,
                    Score = score,
                    TimeSpent = timeSpent,
                    ChapterProgressId = chapterProgress.Id,
                    SubmittedTime = CoreHelper.SystemTimeNow,
                    CreativeAnswer = isCreativeLab ? answerCheck : string.Empty
                };

                await _unitOfWork.GetRepository<Submission>().InsertAsync(submission);
            }

            await _unitOfWork.SaveAsync();

            // update chapterprogress
            await _chapterProgressService.UpdateChapterProgressAsync(chapterProgress.Id, score);
        }
    }
}