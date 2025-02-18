using AutoMapper;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.AuthModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NhaMayMay.Core.Base;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KidPrograming.Services.Infrastructure
{
    public class Authentication
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public Authentication(
              IHttpContextAccessor httpContextAccessor
            , IMapper mapper
             )

        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;

        }
        public async Task<AuthModel> CreateToken(User? user, JwtSettings jwtSettings)
        {
            DateTime now = DateTime.UtcNow;

            // Danh sách các claims chung cho cả Access Token và Refresh Token
            List<Claim> claims = new List<Claim>
                {
                    new Claim("id", user!.Id.ToString()),
                    new Claim("role", user.Role.ToString())
                };
            // đăng kí khóa bảo mật
            SymmetricSecurityKey? key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey ?? string.Empty));
            SigningCredentials? creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Generate access token
            DateTime dateTimeAccessExpr = now.AddMinutes(jwtSettings.AccessTokenExpirationMinutes);
            claims.Add(new Claim("token_type", "access"));
            JwtSecurityToken accessToken = new JwtSecurityToken(
                claims: claims,
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                expires: dateTimeAccessExpr,
                signingCredentials: creds
            );
            string accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);
            AuthModel response = new AuthModel
            {
                Token = accessTokenString,
                UserResponse = _mapper.Map<UserResponse>(user)
            };
            return response;
        }
    }
}

