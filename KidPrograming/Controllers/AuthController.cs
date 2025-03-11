using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core.Base;
using KidPrograming.Core.Constants;
using KidProgramming.ModelViews.ModelViews.AuthModel;
using Microsoft.AspNetCore.Mvc;
namespace KidPrograming.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("login-google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var model = await _authenticationService.Login(request);
            return Ok(BaseResponseModel<string>.OkDataResponse(model, "Login successfully"));
        }

        [HttpGet]
        [Route("infor")]
        public async Task<IActionResult> GetUserInfo()
        {
            ResponseUserModel model = await _authenticationService.GetUserInfo();
            return Ok(BaseResponseModel<string>.OkDataResponse(model, "Retrieve user info successfully"));
        }
        [HttpGet]
        [Route("get-all")]
        public async Task<IActionResult> GetAllUser(string? searchById, Enums.Role? searchByRole, string? searchKeyword, int pageIndex = 1, int pageSize = 10)
        {
            List<ResponseUserModel> model = await _authenticationService.GetAllUser(searchById, searchByRole, searchKeyword, pageIndex = 1, pageSize = 10);
            return Ok(BaseResponseModel<string>.OkDataResponse(model, "Retrieve user info successfully"));
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            ResponseUserModel model = await _authenticationService.GetUserById(id);
            return Ok(BaseResponseModel<string>.OkDataResponse(model, "Retrieve user info successfully"));
        }


        [HttpPut]
        [Route("user-update-infor")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel request)
        {
            ResponseUserModel model = await _authenticationService.UpdateUserInfo(request);
            return Ok(BaseResponseModel<string>.OkDataResponse(model, "Updated successfully"));
        }

    }
}
