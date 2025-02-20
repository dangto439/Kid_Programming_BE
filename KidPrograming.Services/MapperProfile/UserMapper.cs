using AutoMapper;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.AuthModel;

namespace KidPrograming.Services.MapperProfile
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<User, UserResponse>();
            CreateMap<User, ResponseUserModel>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

        }

    }
}
