using AutoMapper;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Repositories.PaggingItems;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core;
using KidPrograming.Core.Constants;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.CourseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


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

            bool teacherExists = await _unitOfWork.GetRepository<User>().Entities.AnyAsync(x => x.Id == model.TeacherId);

            if (!teacherExists)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Teacher not found");
            }

            bool exists = await _unitOfWork.GetRepository<Course>().Entities.AnyAsync(x => x.Title == model.Title && !x.DeletedTime.HasValue);

            if (exists)
            {
                throw new ErrorException(StatusCodes.Status409Conflict, ResponseCodeConstants.EXISTED, $"Title {nameof(model.Title)} has existed");
            }

            Course newCourse = _mapper.Map<Course>(model);

            newCourse.Status = Enums.CourseStatus.Active;
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

        public async Task<PaginatedList<ResponseCourseModel>> GetPage(bool? sortByTitle, bool? sortByPrice, Enums.CourseStatus? filterByStatus, string? searchById, string? searchByTitle, string? searchBySubject, string? teacherName, decimal? minPrice, decimal? maxPrice, int index, int pageSize)
        {
            IQueryable<ResponseCourseModel> query = from course in _unitOfWork.GetRepository<Course>().Entities
                                                    join user in _unitOfWork.GetRepository<User>().Entities
                                                    on course.TeacherId equals user.Id into userGroup
                                                    from user in userGroup.DefaultIfEmpty()
                                                    where !course.DeletedTime.HasValue
                                                    select new ResponseCourseModel
                                                    {
                                                        Id = course.Id,
                                                        Title = course.Title,
                                                        Description = course.Description,
                                                        Subject = course.Subject,
                                                        Price = course.Price ?? 0,
                                                        ThumbnailUrl = course.ThumbnailUrl ?? null,
                                                        Status = course.Status,
                                                        TeacherName = user.FullName ?? "N/A",
                                                        CreatedTime = course.CreatedTime
                                                    };
            if (!string.IsNullOrWhiteSpace(searchById))
            {
                searchById = searchById.Trim();
                query = query.Where(x => EF.Functions.Like(x.Id, $"%{searchById}%"));

                //query = query.Where(x => x.Title != null && x.Title.ToLower().Contains(searchByTitle));
            }

            if (!string.IsNullOrWhiteSpace(searchByTitle))
            {
                searchByTitle = searchByTitle.Trim();
                query = query.Where(x => EF.Functions.Like(x.Title, $"%{searchByTitle}%"));

                //query = query.Where(x => x.Title != null && x.Title.ToLower().Contains(searchByTitle));
            }

            if (!string.IsNullOrWhiteSpace(searchBySubject))
            {
                searchBySubject = searchBySubject.Trim();
                query = query.Where(x => EF.Functions.Like(x.Subject, $"%{searchBySubject}%"));

                //query = query.Where(x => x.Subject != null && x.Subject.ToLower().Contains(searchBySubject));
            }

            if (filterByStatus.HasValue)
            {
                query = query.Where(x => x.Status == filterByStatus);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(c => c.Price >= minPrice);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(c => c.Price <= maxPrice);
            }

            if (!string.IsNullOrWhiteSpace(teacherName))
            {
                teacherName = teacherName.Trim();
                query = query.Where(x => EF.Functions.Like(x.TeacherName, $"%{teacherName}%"));

                //query = query.Where(c => c.TeacherName.ToLower().Contains(teacherName));
            }

            if (sortByTitle.HasValue && sortByPrice.HasValue)
            {
                query = sortByTitle.Value
                    ? query.OrderBy(c => c.Title).ThenBy(c => c.Price)
                    : query.OrderByDescending(c => c.Title).ThenByDescending(c => c.Price);
            }
            else if (sortByTitle.HasValue)
            {
                query = sortByTitle.Value ? query.OrderBy(c => c.Title) : query.OrderByDescending(c => c.Title);
            }
            else if (sortByPrice.HasValue)
            {
                query = sortByPrice.Value ? query.OrderBy(c => c.Price) : query.OrderByDescending(c => c.Price);
            }


            PaginatedList<ResponseCourseModel> paginatedCourses = await _unitOfWork.GetRepository<ResponseCourseModel>().GetPagging(query, index, pageSize);
            return paginatedCourses;
        }

        public async Task Update(string id, UpdateCourseModel model)
        {
            model.Validate();

            Course course = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(x => x.Id == id && !x.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Course not found");

            _mapper.Map(model, course);

            course.LastUpdatedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<Course>().UpdateAsync(course);
            await _unitOfWork.SaveAsync();
        }
    }
}