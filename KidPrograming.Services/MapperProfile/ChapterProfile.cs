using AutoMapper;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.ChapterModels;
using KidProgramming.ModelViews.ModelViews.LessonModels;

namespace KidPrograming.Services.MapperProfile
{
    public class ChapterProfile : Profile
    {
        public ChapterProfile()
        {
            CreateMap<CreateChapterModel, Chapter>();
            CreateMap<UpdateChapterModel, Chapter>();
        }
    }
}