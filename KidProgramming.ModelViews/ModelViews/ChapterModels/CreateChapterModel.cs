using KidPrograming.Core;
using Microsoft.AspNetCore.Http;

namespace KidProgramming.ModelViews.ModelViews.ChapterModels
{
    public class CreateChapterModel
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
        public string CourseId { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Please enter a title for the chapter.");
            }

            if (Description != null && Description.Length > 1000)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "The description cannot exceed 1000 characters.");
            }

            if (Order < 1)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Order must be greater than 0.");
            }

            if (string.IsNullOrWhiteSpace(CourseId))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Please specify the CourseId.");
            }
        }
    }
}