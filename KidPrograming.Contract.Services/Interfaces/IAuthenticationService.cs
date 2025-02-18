using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.AuthModel;
using NhaMayMay.Core.Base;

namespace KidPrograming.Contract.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<AuthModel> Login(GoogleLoginRequest request);
    }
}
