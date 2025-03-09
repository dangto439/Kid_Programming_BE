using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core.Base;
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

        [HttpPut]
        [Route("/api/auth/user-get-infor")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel request)
        {
            ResponseUserModel model = await _authenticationService.UpdateUserInfo(request);
            return Ok(BaseResponseModel<string>.OkDataResponse(model, "Updated successfully"));
        }
    }
}
