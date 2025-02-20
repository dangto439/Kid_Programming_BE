using KidProgramming.ModelViews.ModelViews.AuthModel;

namespace KidPrograming.Contract.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<AuthModel> Login(GoogleLoginRequest request);
        Task<ResponseUserModel> GetUserInfo();

    }
}
