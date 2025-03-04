using AutoMapper;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core;
using KidPrograming.Entity;
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

        public SubmissionService(IUnitOfWork unitOfWork, IMapper mapper, IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _authenticationService = authenticationService;
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
            var lab = await _unitOfWork.GetRepository<Lab>().Entities
        .FirstOrDefaultAsync(x => x.Id == model.LabId && x.DeletedTime == null);
            if (lab == null)
            {
                throw new KeyNotFoundException("Lab not found or has been deleted");
            }

            var chapterProgress = await _unitOfWork.GetRepository<ChapterProgress>().Entities
                .FirstOrDefaultAsync(x => x.Id == model.ChapterProgressId && x.DeletedTime == null);
            if (chapterProgress == null)
            {
                throw new KeyNotFoundException("ChapterProgress not found or has been deleted");
            }
            var user = _authenticationService.GetUserInfo();
            var submission = _mapper.Map<Submission>(model);
            submission.UserId = user.Result.Id!;
            submission.SubmittedTime = DateTimeOffset.UtcNow;
            submission.CreatedTime = DateTimeOffset.UtcNow;

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

            if (score > 100 || score < 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Score must be ranges from 0 to 100");
            }

            submission.Score = score;
            submission.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Submission>().UpdateAsync(submission);
            await _unitOfWork.SaveAsync();
        }
    }
}
