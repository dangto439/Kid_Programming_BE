﻿using AutoMapper;
using FirebaseAdmin.Auth;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Core;
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

        public async Task<List<ResponseUserModel>> GetAllUser(
            string? searchById,
            Enums.Role? searchByRole,
            string? searchByName,
            int pageIndex = 1,
            int pageSize = 10)
        {
            var query = _unitOfWork.GetRepository<User>().Entities
                .Where(x => !x.DeletedTime.HasValue);

            if (!string.IsNullOrEmpty(searchById))
            {
                query = query.Where(x => x.Id.Equals(searchById));
            }

            if (!string.IsNullOrEmpty(searchByName))
            {
                query = query.Where(x => x.FullName!.ToLower().Contains(searchByName.ToLower()));
            }

            if (searchByRole.HasValue)
            {
                string roleString = Enum.GetName(typeof(Enums.Role), searchByRole.Value);
                query = query.Where(x => x.Role == roleString);
            }

            query = query.OrderByDescending(x => x.CreatedTime);
            var users = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            var response = _mapper.Map<List<ResponseUserModel>>(users);

            return response;
        }

        public async Task<ResponseUserModel> GetUserById(string id)
        {
            User user = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(x => x.Id == id && !x.DeletedTime.HasValue) ??
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "User not found");

            return _mapper.Map<ResponseUserModel>(user);
        }

        public async Task<ResponseUserModel> GetUserInfo()
        {
            string userId = _authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
            User user = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(user => user.Id == userId)
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "User not found");


            return _mapper.Map<ResponseUserModel>(user);
        }
        public async Task<AuthModel> Login(GoogleLoginRequest request)
        {

            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.IdToken);
            string uid = decodedToken.Uid; // ID của user
            string email = decodedToken.Claims["email"].ToString()!;
            string name = decodedToken.Claims["name"].ToString()!;
            string picture = decodedToken.Claims["picture"].ToString()!;

            var user = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(user => user.Email.Equals(email));
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    FullName = name,
                    Role = Enums.Role.Customer.ToString(),
                    AvatarUrl = picture
                };
                await _unitOfWork.GetRepository<User>().InsertAsync(user);
                await _unitOfWork.GetRepository<User>().SaveAsync();
            }

            if (!string.IsNullOrWhiteSpace(request.FcmToken))
            {
                user.DeviceToken = request.FcmToken;
                await _unitOfWork.GetRepository<User>().UpdateAsync(user);
                await _unitOfWork.SaveAsync();
            }

               
                

            return await _authentication.CreateToken(user, _jwtSettings);
        }

        public async Task<ResponseUserModel> UpdateUserInfo(UpdateUserModel request)
        {
            string userId = _authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

            User user = await _unitOfWork.GetRepository<User>().Entities
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "User not found");

            if (!string.IsNullOrEmpty(request.PhoneNumber) && request.PhoneNumber != user.PhoneNumber)
            {
                bool phoneExists = await _unitOfWork.GetRepository<User>().Entities
                    .AnyAsync(u => u.PhoneNumber == request.PhoneNumber && u.Id != userId && !u.DeletedTime.HasValue);

                if (phoneExists)
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Phone number is already in use.");
                }
            }

            if (request.ParentId != user.ParentId)
            {
                if (!string.IsNullOrEmpty(request.ParentId))
                {
                    bool parentExists = await _unitOfWork.GetRepository<User>().Entities
                        .AnyAsync(u => u.Id == request.ParentId && u.Role == Enums.Role.Parent.ToString() && !u.DeletedTime.HasValue);
                    if (!parentExists)
                    {
                        throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Parent ID does not exist or was deleted.");
                    }
                }
            }

            _mapper.Map(request, user);

            user.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<User>().UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<ResponseUserModel>(user);
        }
    }
}

