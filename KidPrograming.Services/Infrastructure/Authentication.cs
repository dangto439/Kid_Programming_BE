using AutoMapper;
using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Core;
using KidPrograming.Core.Base;
using KidPrograming.Entity;
using KidProgramming.ModelViews.ModelViews.AuthModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KidPrograming.Services.Infrastructure
{
    public class Authentication
    {

        private readonly IMapper _mapper;
        public Authentication(IMapper mapper)
        {

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
        public  string GetUserIdFromHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            IUnitOfWork unitOfWork = httpContextAccessor.HttpContext!.RequestServices.GetRequiredService<IUnitOfWork>();;
            try
            {
                if (httpContextAccessor.HttpContext == null || !httpContextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    throw new UnauthorizedException("Need Authorization");
                }

                string? authorizationHeader = httpContextAccessor.HttpContext.Request.Headers["Authorization"];

                if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    throw new UnauthorizedException($"Invalid authorization header: {authorizationHeader}");
                }

                string jwtToken = authorizationHeader["Bearer ".Length..].Trim();

                var tokenHandler = new JwtSecurityTokenHandler();

                if (!tokenHandler.CanReadToken(jwtToken))
                {
                    throw new UnauthorizedException("Invalid token format");
                }

                var token = tokenHandler.ReadJwtToken(jwtToken);
                var idClaim = token.Claims.FirstOrDefault(claim => claim.Type == "id");

                return idClaim?.Value ?? "Unknow";
            }
            catch (UnauthorizedException ex)
            {
                var errorResponse = new
                {
                    data = "An unexpected error occurred.",
                    message = ex.Message,
                    statusCode = StatusCodes.Status401Unauthorized,
                    code = "Unauthorized!"
                };

                var jsonResponse = System.Text.Json.JsonSerializer.Serialize(errorResponse);

                if (httpContextAccessor.HttpContext != null)
                {
                    httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    httpContextAccessor.HttpContext.Response.ContentType = "application/json";
                    httpContextAccessor.HttpContext.Response.WriteAsync(jsonResponse).Wait();
                }

                httpContextAccessor.HttpContext?.Response.WriteAsync(jsonResponse).Wait();

                throw;
            }
        }
    }
}

