using AutoMapper;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.SubmissionModels;

namespace KidPrograming.Services.MapperProfile
{
    public class SubmissionProfile : Profile
    {
        public SubmissionProfile() 
        {
            CreateMap<Submission, ResponseSubmissionModel>();
            CreateMap<CreateSubmissionModel, Submission>();
            CreateMap<UpdateSubmissionModel, Submission>();
        }
    }
}
