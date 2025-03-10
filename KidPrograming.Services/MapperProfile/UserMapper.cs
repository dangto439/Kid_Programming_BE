using AutoMapper;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.AuthModel;
using KidProgramming.ModelViews.ModelViews.EnrollmentModels;

namespace KidPrograming.Services.MapperProfile
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<User, UserResponse>();
            CreateMap<User, ResponseUserModel>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
            CreateMap<User, StudentModel>();
            CreateMap<UpdateUserModel, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        }

    }
}

