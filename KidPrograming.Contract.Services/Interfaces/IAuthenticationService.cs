using Azure.Core;
using KidProgramming.ModelViews.ModelViews.AuthModel;

namespace KidPrograming.Contract.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<AuthModel> Login(GoogleLoginRequest request);
        Task<ResponseUserModel> GetUserInfo();
        Task<ResponseUserModel> UpdateUserInfo(UpdateUserModel request);
        Task<ResponseUserModel> GetUserById(string id);

    }
}