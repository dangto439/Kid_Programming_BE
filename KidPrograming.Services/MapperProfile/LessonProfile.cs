using AutoMapper;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.LessonModels;

namespace KidPrograming.Services.MapperProfile
{
    public class LessonProfile : Profile
    {
        public LessonProfile()
        {
            CreateMap<Lesson, ResponseLessonModel>();
            CreateMap<CreateLessonModel, Lesson>();
            CreateMap<UpdateLessonModel, Lesson>();
        }
    }
}