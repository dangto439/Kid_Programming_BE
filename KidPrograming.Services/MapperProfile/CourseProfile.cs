using AutoMapper;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.CourseModels;

namespace KidPrograming.Services.MapperProfile
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<CreateCourseModel, Course>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src =>
                string.IsNullOrWhiteSpace(src.TeacherId) ? null : src.TeacherId
            )).ReverseMap();

            CreateMap<UpdateCourseModel, Course>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedTime, opt => opt.Ignore()) 
            .ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore()) 
            .ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src =>
                string.IsNullOrWhiteSpace(src.TeacherId) ? null : src.TeacherId
            ));

            //CreateMap<Course, CreateCourseModel>();
        }
    }
}