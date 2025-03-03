using AutoMapper;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.LabModels;

namespace KidPrograming.Services.Infrastructure
{
    public class LabProfile : Profile
    {
        public LabProfile()
        {
            CreateMap<Lab, ResponseLabModel>();
            CreateMap<CreateLabModel, Lab>();
            CreateMap<UpdateLabModel, Lab>();
        }
    }
}
