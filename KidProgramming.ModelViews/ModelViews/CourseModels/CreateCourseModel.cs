using KidPrograming.Core;
using Microsoft.AspNetCore.Http;

namespace KidProgramming.ModelViews.ModelViews.CourseModels
{
    public class CreateCourseModel
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Subject { get; set; }
        public decimal? Price { get; set; }
        public string? TeacherId { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Please enter a title for the course");
            }


            if (!string.IsNullOrEmpty(Description) && Description.Length > 1000)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Please do not enter a description longer than 1000 characters");
            }

            if (string.IsNullOrWhiteSpace(Subject))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Please enter a subject name for the course");
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