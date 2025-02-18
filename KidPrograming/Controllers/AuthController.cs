using KidPrograming.Contract.Services.Interfaces;
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
            return Ok(await _authenticationService.Login(request));
        }
    }
}
