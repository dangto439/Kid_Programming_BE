using Google.Apis.Auth;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core;
using KidPrograming.Entity;
using KidPrograming.Repositories.Repositories;
using KidPrograming.Services.Infrastructure;
using KidProgramming.ModelViews.ModelViews.AuthModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using NhaMayMay.Core.Base;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken) ??  throw new Exception("Invalid Google token."); ;
                string email = payload.Email;
                string name = payload.Name;
                string googleId = payload.Subject;

            var user = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(user => user.Email.Equals(email));
            if (user == null)
                {
                    user = new User { 
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
