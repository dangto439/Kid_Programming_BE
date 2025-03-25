using AutoMapper;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.LabModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace KidPrograming.Services.Services
{
    public class LabService : ILabService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LabService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedList<ResponseLabModel>> GetPageAsync(
            string? searchById,
            string chapterId,
            string? searchByTitle,
            string? searchByQuestion,
            bool? sortByTitle,
            bool? sortByResult,
            int index,
            int pageSize)
        {
            var query = _unitOfWork.GetRepository<Lab>().Entities
                .Where(l => l.DeletedTime == null);

            if (!string.IsNullOrEmpty(searchById))
            {
                query = query.Where(l => l.Id == searchById);
            }

            if (!string.IsNullOrEmpty(chapterId))
            {
                query = query.Where(l => l.ChapterId == chapterId);
            }

            if (!string.IsNullOrEmpty(searchByTitle))
            {
                query = query.Where(l => l.Title.Contains(searchByTitle));
            }

            if (!string.IsNullOrEmpty(searchByQuestion))
            {
                query = query.Where(l => l.Question.Contains(searchByQuestion));
            }

            if (sortByTitle == true)
            {
                query = query.OrderBy(l => l.Title);
            }
            else if (sortByTitle == false)
            {
                query = query.OrderByDescending(l => l.Title);
            }

            if (sortByResult == true)
            {
                query = query.OrderBy(l => l.Result);
            }
            else if (sortByResult == false)
            {
                query = query.OrderByDescending(l => l.Result);
            }

            var mappedQuery = query.Select(l => _mapper.Map<ResponseLabModel>(l));
            return await _unitOfWork.GetRepository<ResponseLabModel>().GetPagging(mappedQuery, index, pageSize);
        }

        public async Task CreateAsync(CreateLabModel model)
        {
            model.Validate();

            bool chapterExist = await _unitOfWork.GetRepository<Chapter>().Entities.AnyAsync(x => x.Id == model.ChapterId && !x.DeletedTime.HasValue);

            if (!chapterExist)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Chapter not found");
            }

            Lab? existingLab = await _unitOfWork.GetRepository<Lab>().Entities
                .FirstOrDefaultAsync(l => l.ChapterId == model.ChapterId);

            if (existingLab != null)
            {
                if (existingLab.DeletedTime.HasValue)
                {
                    existingLab.Question = model.Question;
                    existingLab.Title = model.Title;
                    existingLab.Description = model.Description;
                    existingLab.Result = model.Result;
                    existingLab.Type = model.Type.ToString();
                    existingLab.LimitedTime = model.LimitedTime;
                    existingLab.CorrectAnswer = model.CorrectAnswer;
                    existingLab.ToolBox = model.ToolBox;
                    existingLab.Solution = model.Solution;
                    existingLab.ChapterId = model.ChapterId;
                    existingLab.DeletedTime = null;

                    await _unitOfWork.GetRepository<Lab>().UpdateAsync(existingLab);
                    await _unitOfWork.SaveAsync();
                    return;
                }

                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Each chapter only has 1 single lab");
            }

            Lab lab = _mapper.Map<Lab>(model);

            lab.CreatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Lab>().InsertAsync(lab);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(string id, UpdateLabModel model)
        {
            model.Validate();

            Lab lab = await _unitOfWork.GetRepository<Lab>().Entities.FirstOrDefaultAsync(x => x.Id == id && !x.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Lab not found");

            bool exists = await _unitOfWork.GetRepository<Lab>().Entities.AnyAsync(x => x.ChapterId == lab.ChapterId && x.Question == model.Question && !x.DeletedTime.HasValue);

            if (exists)
            {
                throw new ErrorException(StatusCodes.Status409Conflict, ResponseCodeConstants.EXISTED, $"Lab {(model.Question)} has duplicated");
            }

            lab.LastUpdatedTime = CoreHelper.SystemTimeNow;

            _mapper.Map(model, lab);

            await _unitOfWork.GetRepository<Lab>().UpdateAsync(lab);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(string id)
        {
            Lab lab = await _unitOfWork.GetRepository<Lab>().Entities.FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Lab not found");

            lab.DeletedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Lab>().UpdateAsync(lab);
            await _unitOfWork.SaveAsync();
        }

        public async Task<string> GetAnswerByLabIdAsync(string id)
        {
            Lab lab = await _unitOfWork.GetRepository<Lab>().GetByIdAsync(id) ??
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Lab not found");

            return lab.CorrectAnswer;
        }
    }
}
