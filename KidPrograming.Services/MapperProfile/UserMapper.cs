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
        }

    }
}
