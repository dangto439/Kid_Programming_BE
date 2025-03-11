using Azure.Core;
using KidPrograming.Core.Constants;
using KidProgramming.ModelViews.ModelViews.AuthModel;

namespace KidPrograming.Contract.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<AuthModel> Login(GoogleLoginRequest request);
        Task<ResponseUserModel> GetUserInfo();
        Task<ResponseUserModel> UpdateUserInfo(UpdateUserModel request);
        Task<ResponseUserModel> GetUserById(string id);
        Task<List<ResponseUserModel>> GetAllUser(string? searchById, Enums.Role? searchByRole, string? searchByName,int pageIndex = 1, int pageSize = 10);

    }
}