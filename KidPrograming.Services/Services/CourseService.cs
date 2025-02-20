using AutoMapper;
using KidPrograming.Entity;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Services.Interfaces;
using KidProgramming.ModelViews.ModelViews.CourseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using KidPrograming.Core;

namespace KidPrograming.Services.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CourseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task Create(CreateCourseModel model)
        {
            model.Validate();

            bool exists = await _unitOfWork.GetRepository<Course>().Entities.AnyAsync(x => x.Title == model.Title && !x.DeletedTime.HasValue);

            if(exists)
            {
                throw new ErrorException(StatusCodes.Status409Conflict, ResponseCodeConstants.EXISTED, $"Title {nameof(model.Title)} has existed");
            }

            Course newCourse = _mapper.Map<Course>(model);

            newCourse.CreatedTime = CoreHelper.SystemTimeNow;
            newCourse.LastUpdatedTime = CoreHelper.SystemTimeNow;

            newCourse.Price ??= 0;

            await _unitOfWork.GetRepository<Course>().InsertAsync(newCourse);
            await _unitOfWork.SaveAsync();
        }

        public async Task Delete(string id)
        {
            Course course = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(x => x.Id == id && !x.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Course not found");
            
            course.DeletedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<Course>().UpdateAsync(course);
            await _unitOfWork.SaveAsync();
        }

        public async Task Update(string id, UpdateCourseModel model)
        {
            throw new NotImplementedException();
        }
    }
}