using AutoMapper;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.ChapterModels;

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