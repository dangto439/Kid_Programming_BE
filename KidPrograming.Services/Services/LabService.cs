using AutoMapper;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.LabModels;
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
            string? lessonId = null,
            string? searchByTitle = null,
            string? searchByQuestion = null,
            bool? sortByTitle = null,
            bool? sortByResult = null,
            int index = 1,
            int pageSize = 10)
        {
            var query = _unitOfWork.GetRepository<Lab>().Entities
                .Where(l => l.DeletedTime == null);

            if (!string.IsNullOrEmpty(lessonId))
            {
                query = query.Where(l => l.LessionId == lessonId);
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
            var lesson = _unitOfWork.GetRepository<Lesson>().Entities.FirstOrDefault(x => x.Id == model.LessionId);
            if (lesson == null)
            {
                throw new KeyNotFoundException("Lesson not found or has been deleted");
            }
            var lab = _mapper.Map<Lab>(model);
            lab.CreatedTime = DateTime.Now;
            await _unitOfWork.GetRepository<Lab>().InsertAsync(lab);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(string id, UpdateLabModel model)
        {
            model.Validate();
            var lab = await _unitOfWork.GetRepository<Lab>().Entities
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null);

            if (lab == null)
                throw new KeyNotFoundException("Lab not found or has been deleted");

            lab.LastUpdatedTime = DateTime.Now;
            _mapper.Map(model, lab);
            await _unitOfWork.GetRepository<Lab>().UpdateAsync(lab);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var lab = await _unitOfWork.GetRepository<Lab>().Entities
                .FirstOrDefaultAsync(x => x.Id == id);

            if (lab == null)
                throw new KeyNotFoundException("Lab not found");

            if (lab.DeletedTime != null)
                throw new InvalidOperationException("Lab has already been deleted");

            lab.DeletedTime = DateTime.Now;
            await _unitOfWork.GetRepository<Lab>().UpdateAsync(lab);
            await _unitOfWork.SaveAsync();
        }
    }
}
