using AutoMapper;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.ChapterProgressModels;
using KidProgramming.ModelViews.ModelViews.LessonModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidPrograming.Services.MapperProfile
{
    public class ChapterProgressProfile : Profile
    {
        public ChapterProgressProfile()
        {
            CreateMap<ChapterProgress, ResponseChapterProgressModel>();
            CreateMap<CreateChapterProgressModel, ChapterProgress>();
            CreateMap<UpdateChapterProgressModel, ChapterProgress>();
            CreateMap<Submission, LabScore>()
                     .ForMember(dest => dest.LabTile, opt => opt.MapFrom(src => src.Lab.Title))
                     .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score));
        }
    }
}
