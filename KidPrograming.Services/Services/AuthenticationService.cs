using AutoMapper;
using FirebaseAdmin.Auth;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core;
using KidPrograming.Entity;
using KidPrograming.Services.Infrastructure;
using KidProgramming.ModelViews.ModelViews.AuthModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NhaMayMay.Core.Base;

namespace KidPrograming.Services.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Authentication _authentication;
        private readonly IMapper _mapper;

        public AuthenticationService(
              IUnitOfWork unitOfWork
            , JwtSettings jwtSettings
            , IHttpContextAccessor httpContextAccessor
           , Authentication authentication
            , IMapper mapper
            )

        {
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtSettings;
            _httpContextAccessor = httpContextAccessor;
            _authentication = authentication;
            _mapper = mapper;

        }
        public async Task<ResponseUserModel> GetUserInfo()
        {
            string userId = _authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
            User user = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(user => user.Id == userId)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Không tìm thấy người dùng nào.");


            return _mapper.Map<ResponseUserModel>(user);
        }
        public async Task<AuthModel> Login(GoogleLoginRequest request)
        {

            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.IdToken);
            string uid = decodedToken.Uid; // ID của user
            string email = decodedToken.Claims["email"].ToString()!;
            string name = decodedToken.Claims["name"].ToString()!;

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


