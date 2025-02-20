using Google.Apis.Auth;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core.Base;
using KidPrograming.Core.Constants;
using KidPrograming.Entity;
using KidPrograming.Services.Infrastructure;
using KidProgramming.ModelViews.ModelViews.AuthModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace KidPrograming.Services.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Authentication _authentication;

        public AuthenticationService(
               IUnitOfWork unitOfWork
             , JwtSettings jwtSettings
             , IHttpContextAccessor httpContextAccessor
            , Authentication authentication
             )

        {
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtSettings;
            _httpContextAccessor = httpContextAccessor;
            _authentication = authentication;
        }

        public async Task<AuthModel> Login(GoogleLoginRequest request)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken) ?? throw new Exception("Invalid Google token."); ;
            string email = payload.Email;
            string name = payload.Name;
            string googleId = payload.Subject;

            var user = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(user => user.Email.Equals(email));
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    FullName = name,
                    Role = Enums.Role.Customer
                };
                await _unitOfWork.GetRepository<User>().InsertAsync(user);
                await _unitOfWork.GetRepository<User>().SaveAsync();
            }
            return await _authentication.CreateToken(user, _jwtSettings);
        }
    }
}