using KidPrograming.Core;
using Microsoft.AspNetCore.Http;
using static KidPrograming.Core.Constants.Enums;

namespace KidProgramming.ModelViews.ModelViews.CourseModels
{
    public class UpdateCourseModel
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Subject { get; set; }
        public string thumbnailUrl { get; set; }
        public decimal? Price { get; set; }
        public string? TeacherId { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Please enter a title for the course");
            }
            if (string.IsNullOrWhiteSpace(thumbnailUrl))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Please enter a link thumbnail for the course");
            }
            else if (Title.Length > 255)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Please do not enter a title longer than 255 characters");
            }

            if (!string.IsNullOrEmpty(Description) && Description.Length > 500)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Please do not enter a description longer than 255 characters");
            }

            if (string.IsNullOrWhiteSpace(Subject))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Vui lòng nhập tên môn học cho khóa học");
            }
            else if (Subject.Length > 255)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Please do not enter a subject longer than 255 characters");
            }

            if (Price.HasValue)
            {
                if (Price < 0)
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Price cannot be negative.");
                }
                else if (Price > 1_000_000_000)
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Price is too high.");
                }
            }

            if (string.IsNullOrWhiteSpace(TeacherId))
            {
                TeacherId = null;
            }
            else if (!Guid.TryParse(TeacherId, out _))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Invalid TeacherId format. It must be a valid GUID.");
            }
        }
    }
}